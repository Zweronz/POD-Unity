using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PodUnityFactory
{
    public static GameObject LoadPod(string name, PodScene scene)
    {
        GameObject podRoot = new GameObject(name);

        for (int i = 0; i < scene.numNode; i++)
        {
            PodNode node = scene.node[i];

            GameObject objectNode = new GameObject(node.name);

            objectNode.transform.parent = podRoot.transform;

            if (node.animPosition != null && node.animPosition.Length > 0)
            {
                objectNode.transform.localPosition = new Vector3(node.animPosition[0], node.animPosition[1], node.animPosition[2]);
            }

            if (node.animRotation != null && node.animRotation.Length > 0)
            {
                objectNode.transform.localRotation = new Quaternion(-node.animRotation[0], -node.animRotation[1], -node.animRotation[2], node.animRotation[3]);
            }

            if (node.animScale != null && node.animScale.Length > 0)
            {
                objectNode.transform.localScale = new Vector3(node.animScale[0], node.animScale[1], node.animScale[2]);
            }

            if (node.animMatrix != null && node.animMatrix.Length > 0)
            {
                Matrix4x4 matrix = new Matrix4x4
                (
                    new Vector4(node.animMatrix[0], node.animMatrix[1], node.animMatrix[2], node.animMatrix[3]),
                    new Vector4(node.animMatrix[4], node.animMatrix[5], node.animMatrix[6], node.animMatrix[7]),
                    new Vector4(node.animMatrix[8], node.animMatrix[9], node.animMatrix[10], node.animMatrix[11]),
                    new Vector4(node.animMatrix[12], node.animMatrix[13], node.animMatrix[14], node.animMatrix[15])
                );

                objectNode.transform.localPosition = matrix.GetColumn(3);
                objectNode.transform.localRotation = Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));

                objectNode.transform.localScale = new Vector3(matrix.GetColumn(0).magnitude, matrix.GetColumn(1).magnitude, matrix.GetColumn(2).magnitude);
            }

            if ((int)node.idxMaterial != -1)
            {
                MeshRenderer renderer = objectNode.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = LoadMaterial(scene, (int)node.idxMaterial);
            }

            if ((int)node.idx != -1)
            {
                MeshFilter filter = objectNode.AddComponent<MeshFilter>();
                filter.sharedMesh = LoadMesh(scene, (int)node.idx);
            }

            cachedNodes.Add(objectNode);
        }

        for (int i = 0; i < cachedNodes.Count; i++)
        {
            PodNode node = scene.node[i];

            if ((int)node.idxParent > -1)
            {
                Vector3 originalPosition = cachedNodes[i].transform.localPosition;
                Quaternion originalRotation = cachedNodes[i].transform.localRotation;
                Vector3 originalScale = cachedNodes[i].transform.localScale;

                cachedNodes[i].transform.parent = cachedNodes[(int)node.idxParent].transform;

                cachedNodes[i].transform.localPosition = originalPosition;
                cachedNodes[i].transform.localRotation = originalRotation;
                cachedNodes[i].transform.localScale = originalScale;
            }

            bool hasMatrixAnim = node.animMatrix != null && node.animMatrix.Length > 16, hasPositionAnim = node.animPosition != null && node.animPosition.Length > 3, hasRotationAnim = node.animRotation != null && node.animRotation.Length > 4, hasScaleAnim = node.animScale != null && node.animScale.Length > 7;
            bool addClip = hasMatrixAnim || hasPositionAnim || hasRotationAnim || hasScaleAnim;

            if (addClip)
            {
                AnimationClip clip;

                if (podRoot.GetComponent<Animation>() == null)
                {
                    clip = new AnimationClip();
                    podRoot.AddComponent<Animation>().clip = clip;
                }
                else
                {
                    clip = podRoot.GetComponent<Animation>().clip;
                }

                List<List<Keyframe>> matrixAnimations = new List<List<Keyframe>>();

                for (int j = 0; j < 10; j++)
                {
                    matrixAnimations.Add(new List<Keyframe>());
                }

                if (hasMatrixAnim)
                {
                    for (int j = 0; j < scene.numFrame; j++)
                    {
                        int index = node.animMatrixIdx != null && node.animMatrixIdx.Length > 0 ? (int)node.animMatrixIdx[j] : j*16;

                        Matrix4x4 animMatrix = GetMatrix(node.animMatrix, index);

                        float time = j == 0 ? 0 : (float)j / scene.fPS;

                        Vector3 position = animMatrix.GetColumn(3);

                        matrixAnimations[0].Add(new Keyframe(time, position.x));
                        matrixAnimations[1].Add(new Keyframe(time, position.y));
                        matrixAnimations[2].Add(new Keyframe(time, position.z));

                        Quaternion rotation = Quaternion.LookRotation(animMatrix.GetColumn(2), animMatrix.GetColumn(1));

                        matrixAnimations[3].Add(new Keyframe(time, rotation.x, 0, 0));
                        matrixAnimations[4].Add(new Keyframe(time, rotation.y, 0, 0));
                        matrixAnimations[5].Add(new Keyframe(time, rotation.z, 0, 0));
                        matrixAnimations[6].Add(new Keyframe(time, rotation.w, 0, 0));

                        Vector3 scale = new Vector3(animMatrix.GetColumn(0).magnitude, animMatrix.GetColumn(1).magnitude, animMatrix.GetColumn(2).magnitude);

                        matrixAnimations[7].Add(new Keyframe(time, scale.x, 0, 0));
                        matrixAnimations[8].Add(new Keyframe(time, scale.y, 0, 0));
                        matrixAnimations[9].Add(new Keyframe(time, scale.z, 0, 0));
                    }
                }

                if (hasPositionAnim)
                {
                    Debug.LogError(name + " pos");
                    for (int j = 0; j < scene.numFrame; j++)
                    {
                        int index = node.animPositionIdx != null && node.animPositionIdx.Length > 0 ? (int)node.animPositionIdx[j] : j*3;
                        float time = j == 0 ? 0 : (float)j / scene.fPS;

                        matrixAnimations[0].Add(new Keyframe(time, node.animPosition[index]));
                        matrixAnimations[1].Add(new Keyframe(time, node.animPosition[index + 1]));
                        matrixAnimations[2].Add(new Keyframe(time, node.animPosition[index + 2]));
                    }
                }

                if (hasRotationAnim)
                {
                    Debug.LogError(name + " rot");
                    for (int j = 0; j < scene.numFrame; j++)
                    {
                        int index = node.animRotationIdx != null && node.animRotationIdx.Length > 0 ? (int)node.animRotationIdx[j] : j*4;
                        float time = j == 0 ? 0 : (float)j / scene.fPS;

                        matrixAnimations[3].Add(new Keyframe(time, -node.animRotation[index]));
                        matrixAnimations[4].Add(new Keyframe(time, -node.animRotation[index + 1]));
                        matrixAnimations[5].Add(new Keyframe(time, -node.animRotation[index + 2]));
                        matrixAnimations[6].Add(new Keyframe(time, node.animRotation[index + 3]));
                    }
                }

                if (hasScaleAnim)
                {
                    Debug.LogError(name + " scl");
                    for (int j = 0; j < scene.numFrame; j++)
                    {
                        int index = node.animScaleIdx != null && node.animScaleIdx.Length > 0 ? (int)node.animScaleIdx[j] : j*7;
                        float time = j == 0 ? 0 : (float)j / scene.fPS;

                        matrixAnimations[7].Add(new Keyframe(time, node.animScale[index]));
                        matrixAnimations[8].Add(new Keyframe(time, node.animScale[index + 1]));
                        matrixAnimations[9].Add(new Keyframe(time, node.animScale[index + 2]));

                        //idk wth to do with the other 4 I have no clue what they do
                    }
                }

                clip.SetCurve(GetGameObjectPath(cachedNodes[i]), typeof(Transform), "localPosition.x", new AnimationCurve(matrixAnimations[0].ToArray()));
                clip.SetCurve(GetGameObjectPath(cachedNodes[i]), typeof(Transform), "localPosition.y", new AnimationCurve(matrixAnimations[1].ToArray()));
                clip.SetCurve(GetGameObjectPath(cachedNodes[i]), typeof(Transform), "localPosition.z", new AnimationCurve(matrixAnimations[2].ToArray()));
                
                clip.SetCurve(GetGameObjectPath(cachedNodes[i]), typeof(Transform), "localRotation.x", new AnimationCurve(matrixAnimations[3].ToArray()));
                clip.SetCurve(GetGameObjectPath(cachedNodes[i]), typeof(Transform), "localRotation.y", new AnimationCurve(matrixAnimations[4].ToArray()));
                clip.SetCurve(GetGameObjectPath(cachedNodes[i]), typeof(Transform), "localRotation.z", new AnimationCurve(matrixAnimations[5].ToArray()));
                clip.SetCurve(GetGameObjectPath(cachedNodes[i]), typeof(Transform), "localRotation.w", new AnimationCurve(matrixAnimations[6].ToArray()));

                clip.SetCurve(GetGameObjectPath(cachedNodes[i]), typeof(Transform), "localScale.x", new AnimationCurve(matrixAnimations[7].ToArray()));
                clip.SetCurve(GetGameObjectPath(cachedNodes[i]), typeof(Transform), "localScale.y", new AnimationCurve(matrixAnimations[8].ToArray()));
                clip.SetCurve(GetGameObjectPath(cachedNodes[i]), typeof(Transform), "localScale.z", new AnimationCurve(matrixAnimations[9].ToArray()));

                clip.legacy = true;
                clip.name = podRoot.name;

                if (podRoot.GetComponent<Animation>().GetClipCount() == 0)
                {
                    podRoot.GetComponent<Animation>().AddClip(clip, podRoot.name);
                }
            }

            if (node.animMatrix != null && node.animMatrix.Length > 16)
            {





            }
        }

        cachedNodes.Clear();
        cachedMeshes.Clear();
        cachedMaterials.Clear();
        cachedTextures.Clear();

        return podRoot;
    }

    private static string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;

        while (obj.transform.parent.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = obj.name + "/" + path;
        }

        return path;
    }

    private static List<GameObject> cachedNodes = new List<GameObject>();

    private static Dictionary<int, Mesh> cachedMeshes = new Dictionary<int, Mesh>();

    private static Dictionary<int, Material> cachedMaterials = new Dictionary<int, Material>();

    private static Dictionary<int, Texture2D> cachedTextures = new Dictionary<int, Texture2D>();

    private static Texture2D LoadTexture(PodScene scene, int index)
    {
        if (cachedTextures.ContainsKey(index))
        {
            return cachedTextures[index];
        }

        Texture2D texture = Resources.Load<Texture2D>(scene.texture[index].name.Split('.')[0]);
        cachedTextures.Add(index, texture);

        return texture;
    }

    private static Material LoadMaterial(PodScene scene, int index)
    {
        if (index == -1)
        {
            return null;
        }

        if (cachedMaterials.ContainsKey(index))
        {
            return cachedMaterials[index];
        }

        PodMaterial podMaterial = scene.material[index];
        Material material = new Material(Shader.Find(podMaterial != null && (int)podMaterial.idxTexOpacity != -1 ? "Legacy Shaders/Transparent/Diffuse" : "Legacy Shaders/Diffuse"))
        {
            mainTexture = LoadTexture(scene, (int)podMaterial.idxTexDiffuse),
            name = podMaterial.name
        };

        cachedMaterials.Add(index, material);

        return material;
    }

    private static Mesh LoadMesh(PodScene scene, int index)
    {
        if (cachedMeshes.ContainsKey(index))
        {
            return cachedMeshes[index];
        }
        
        PodMesh mesh = scene.mesh[index];
        Mesh unityMesh = new Mesh
        {
            vertices = GetVertices(mesh),
            triangles = GetTriangles(mesh),
            normals = GetNormals(mesh),
            uv = GetUVS(mesh),
        };

        cachedMeshes.Add(index, unityMesh);

        return unityMesh;
    }

    private static int[] GetTriangles(PodMesh mesh)
    {
        int[] triangles = new int[mesh.numFaces * 3];
        int index = 0;

        for (int i = 0; i < mesh.numFaces * 6; i+=2)
        {
            triangles[index++] = BitConverter.ToInt16(mesh.faces.data, i);
        }
        
        return triangles;
    }

    private static Vector3[] GetVertices(PodMesh mesh)
    {
        Vector3[] vertices = new Vector3[mesh.numVertex];
        int index = 0;

        for (int i = 0; i < mesh.numVertex * 12; i+=12)
        {
            vertices[index++] = new Vector3(BitConverter.ToSingle(mesh.vertex.data, i), BitConverter.ToSingle(mesh.vertex.data, i + 4), BitConverter.ToSingle(mesh.vertex.data, i + 8));
        }
        
        return vertices;
    }

    private static Vector3[] GetNormals(PodMesh mesh)
    {
        Vector3[] normals = new Vector3[mesh.numVertex];
        int index = 0;

        if (mesh.normals.stride == 0)
        {
            return normals;
        }

        for (int i = 0; i < mesh.numVertex * 12; i+=12)
        {
            normals[index++] = new Vector3(BitConverter.ToSingle(mesh.normals.data, i), BitConverter.ToSingle(mesh.normals.data, i + 4), BitConverter.ToSingle(mesh.normals.data, i + 8));
        }
        
        return normals;
    }

    private static Vector2[] GetUVS(PodMesh mesh)
    {
        Vector2[] uvs = new Vector2[mesh.numVertex];
        int index = 0;

        if (mesh.uVW.Length == 0)
        {
            return uvs;
        }

        for (int i = 0; i < mesh.numVertex * 8; i+=8)
        {
            uvs[index++] = new Vector2(BitConverter.ToSingle(mesh.uVW[0].data, i), -BitConverter.ToSingle(mesh.uVW[0].data, i + 4));
        }
        
        return uvs;
    }

    private static Matrix4x4 GetMatrix(float[] matrix, int startingIdx)
    {
        return new Matrix4x4
        (
            new Vector4(matrix[startingIdx], matrix[startingIdx + 1], matrix[startingIdx + 2], matrix[startingIdx + 3]),
            new Vector4(matrix[startingIdx + 4], matrix[startingIdx + 5], matrix[startingIdx + 6], matrix[startingIdx + 7]),
            new Vector4(matrix[startingIdx + 8], matrix[startingIdx + 9], matrix[startingIdx + 10], matrix[startingIdx + 11]),
            new Vector4(matrix[startingIdx + 12], matrix[startingIdx + 13], matrix[startingIdx + 14], matrix[startingIdx + 15])
        );
    }
}
