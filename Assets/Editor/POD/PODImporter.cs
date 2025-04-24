using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "pod")]
public unsafe class PODImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        SPODScene* scene;

        fixed (byte* path = System.Text.Encoding.UTF8.GetBytes(ctx.assetPath + '\0'))
        {
            scene = PODNative.load_pod_deinterleaved((char*)path);
        }

        /* TODO:
            * means I'm done for now, but support isn't finished

            pfColourBackground
            pfColourAmbient

            pCamera ✔
            pLight ✔*
            pMesh ✔*
            pNode ✔
            pTexture ✔
            pMaterial ✔*

            animations ✔*
            user data
        */
        
        string podName = Path.GetFileNameWithoutExtension(ctx.assetPath);
        GameObject root = new GameObject(podName);

        ctx.AddObjectToAsset("POD_ROOT", root);

        List<SPODNode> nodes = new List<SPODNode>();
        List<GameObject> nodeObjects = new List<GameObject>();

        if (scene->nNumNode > 0)
        {
            for (int i = 0; i < scene->nNumNode; i++)
            {
                nodes.Add(scene->pNode[i]);

                GameObject node = new GameObject(Marshal.PtrToStringUTF8((IntPtr)scene->pNode[i].pszName));
                node.transform.parent = root.transform;

                nodeObjects.Add(node);
            }

            for (int i = 0; i < scene->nNumNode; i++)
            {
                if (scene->pNode[i].nIdxParent > -1)
                {
                    nodeObjects[i].transform.parent = nodeObjects[scene->pNode[i].nIdxParent].transform;
                }
            }

            for (int i = 0; i < scene->nNumNode; i++)
            {
                SyncDefaultPosition(nodeObjects[i], scene->pNode[i]);
                ctx.AddObjectToAsset("POD_NODE_" + i, nodeObjects[i]);
            }
        }

        List<Texture2D> textures = new List<Texture2D>();

        if (scene->nNumTexture > 0)
        {
            List<Texture2D> texturesHere = AssetDatabase.FindAssets("t:Texture2D", new string[1] { Path.GetDirectoryName(ctx.assetPath) })
                .Select(x => AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(x))).ToList();

            List<Texture2D> allTextures = null; // don't search the entire project unless needed.

            for (int i = 0; i < scene->nNumTexture; i++)
            {
                string texturePath = Marshal.PtrToStringUTF8((IntPtr)scene->pTexture[i].pszName);
                string textureName = Path.GetFileNameWithoutExtension(texturePath);

                Texture2D target = texturesHere.Find(x => x.name == textureName);

                if (target == null)
                {
                    if (allTextures == null)
                    {
                        allTextures = AssetDatabase.FindAssets("t:Texture2D").Select(x =>
                            AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(x))).ToList();
                    }
                    
                    target = allTextures.Find(x => x.name == textureName);

                    if (target == null)
                    {
                        Debug.LogWarning("Error importing " + podName + ": " + texturePath + " could not be found!");
                    }
                }

                textures.Add(target);
            }
        }

        List<Material> materials = new List<Material>();

        if (scene->nNumMaterial > 0)
        {
            for (int i = 0; i < scene->nNumMaterial; i++)
            {
                Material material = new Material(Shader.Find("Legacy Shaders/Diffuse")); // placeholder
                material.name = Marshal.PtrToStringUTF8((IntPtr)scene->pMaterial[i].pszName);

                if (scene->pMaterial[i].nIdxTexDiffuse > -1)
                {
                    material.mainTexture = textures[scene->pMaterial[i].nIdxTexDiffuse];

                    if (textures[scene->pMaterial[i].nIdxTexDiffuse] != null && textures[scene->pMaterial[i].nIdxTexDiffuse].alphaIsTransparency)
                    {
                        material.shader = Shader.Find("Legacy Shaders/Transparent/Diffuse"); // placeholder
                    }

                    materials.Add(material);
                }

                ctx.AddObjectToAsset("POD_MATERIAL_" + i, material);

                // unless we have an accurate shader to go off of, there's not much we can
                // do with the rest of the values ¯\_(ツ)_/¯
            }
        }

        List<Mesh> meshes = new List<Mesh>();

        if (scene->nNumMesh > 0)
        {
            for (int i = 0; i < scene->nNumMesh; i++)
            {
                SPODMesh podMesh = scene->pMesh[i];
                Mesh mesh = new Mesh();

                if (podMesh.sVertex.n > 0)
                {
                    if (podMesh.sVertex.n == 3 && podMesh.sVertex.eType == EPVRTDataType.EPODDataFloat)
                    {
                        Vector3[] vertices = new Vector3[podMesh.nNumVertex];

                        for (uint j = 0; j < podMesh.nNumVertex; j++)
                        {
                            vertices[j] = PODConvert.ToUnityVector3((float*)podMesh.sVertex.pData, j * 3);
                        }

                        mesh.SetVertices(vertices);
                    }
                    else
                    {
                        Debug.LogError(podName + " mesh num " + i + "'s vertex format (" + podMesh.sVertex.eType.ToString() + ", " + podMesh.sVertex.n + ") does not match any implementation!");
                    }
                }

                if (podMesh.sNormals.n > 0)
                {
                    if (podMesh.sNormals.n == 3 && podMesh.sNormals.eType == EPVRTDataType.EPODDataFloat)
                    {
                        Vector3[] normals = new Vector3[podMesh.nNumVertex];

                        for (uint j = 0; j < podMesh.nNumVertex; j++)
                        {
                            normals[j] = PODConvert.ToUnityVector3((float*)podMesh.sNormals.pData, j * 3);
                        }

                        mesh.SetNormals(normals);
                    }
                    else
                    {
                        Debug.LogError(podName + " mesh num " + i + "'s normal format (" + podMesh.sNormals.eType.ToString() + ", " + podMesh.sNormals.n + ") does not match any implementation!");
                    }
                }

                if (podMesh.sTangents.n > 0)
                {
                    if (podMesh.sTangents.n == 3 && podMesh.sTangents.eType == EPVRTDataType.EPODDataFloat)
                    {
                        Vector4[] tangents = new Vector4[podMesh.nNumVertex];

                        for (uint j = 0; j < podMesh.nNumVertex; j++)
                        {
                            tangents[j] = (Vector4)PODConvert.ToUnityVector3((float*)podMesh.sTangents.pData, j * 3);
                        }

                        mesh.SetTangents(tangents);
                    }
                    else
                    {
                        Debug.LogError(podName + " mesh num " + i + "'s tangent format (" + podMesh.sTangents.eType.ToString() + ", " + podMesh.sTangents.n + ") does not match any implementation!");
                    }
                }

                // unity does not allow you to set binormals

                if (podMesh.sVtxColours.n > 0)
                {
                    if (podMesh.sVtxColours.n == 4 && (podMesh.sVtxColours.eType == EPVRTDataType.EPODDataRGBA || podMesh.sVtxColours.eType == EPVRTDataType.EPODDataARGB))
                    {
                        Color32[] colors = new Color32[podMesh.nNumVertex];

                        if (podMesh.sVtxColours.eType == EPVRTDataType.EPODDataRGBA)
                        {
                            int index = 0;

                            for (int j = 0; j < podMesh.nNumVertex; j++)
                            {
                                colors[j] = new Color32(podMesh.sVtxColours.pData[index], podMesh.sVtxColours.pData[index + 1],
                                    podMesh.sVtxColours.pData[index + 2], podMesh.sVtxColours.pData[index + 3]);

                                index += 4;
                            }
                        }
                        else if (podMesh.sVtxColours.eType == EPVRTDataType.EPODDataARGB)
                        {
                            int index = 0;

                            for (int j = 0; j < podMesh.nNumVertex; j++)
                            {
                                colors[j] = new Color32(podMesh.sVtxColours.pData[index + 3], podMesh.sVtxColours.pData[index],
                                    podMesh.sVtxColours.pData[index + 1], podMesh.sVtxColours.pData[index + 2]);
                                    
                                index += 4;
                            }
                        }

                        mesh.SetColors(colors);
                    }
                    else
                    {
                        Debug.LogError(podName + " mesh num " + i + "'s color format (" + podMesh.sVtxColours.eType.ToString() + ", " + podMesh.sVtxColours.n + ") does not match any implementation!");
                    }
                }

                if (podMesh.nNumUVW > 0)
                {
                    for (int j = 0; j < podMesh.nNumUVW; j++)
                    {
                        if (podMesh.psUVW[j].n == 2 && podMesh.psUVW[j].eType == EPVRTDataType.EPODDataFloat)
                        {
                            Vector2[] uvs = new Vector2[podMesh.nNumVertex];

                            for (uint k = 0; k < podMesh.nNumVertex; k++)
                            {
                                uvs[k] = PODConvert.ToUnityVector2((float*)podMesh.psUVW[j].pData, k * 2);
                            }

                            mesh.SetUVs(j, uvs);
                        }
                        else
                        {
                            Debug.LogError(podName + " mesh num " + i + "'s uv" + j + " format (" + podMesh.psUVW[j].eType.ToString() + ", " + podMesh.psUVW[j].n + ") does not match any implementation");
                        }
                    }
                }

                if (podMesh.nNumFaces > 0)
                {
                    int[] triangles = new int[podMesh.nNumFaces * 3];

                    for (int j = 0; j < podMesh.nNumFaces * 3; j++)
                    {
                        if (podMesh.sFaces.eType == EPVRTDataType.EPODDataUnsignedByte || podMesh.sFaces.eType == EPVRTDataType.EPODDataByte)
                        {
                            triangles[j] = podMesh.sFaces.pData[j];
                        }
                        else if (podMesh.sFaces.eType == EPVRTDataType.EPODDataUnsignedShort || podMesh.sFaces.eType == EPVRTDataType.EPODDataShort)
                        {
                            triangles[j] = ((ushort*)podMesh.sFaces.pData)[j];
                        }
                        else if (podMesh.sFaces.eType == EPVRTDataType.EPODDataUnsignedInt || podMesh.sFaces.eType == EPVRTDataType.EPODDataInt)
                        {
                            triangles[j] = (int)((uint*)podMesh.sFaces.pData)[j];
                        }
                        else
                        {
                            Debug.LogError(podName + " mesh num " + i + "'s index format (" + podMesh.sFaces.eType.ToString() + ") does not match any implementation!");
                            break;
                        }
                    }

                    mesh.SetTriangles(triangles, 0);
                }

                ctx.AddObjectToAsset("POD_MESH_" + i, mesh);
                mesh.RecalculateBounds();

                meshes.Add(mesh);
            }

            for (int i = 0; i < scene->nNumNode; i++)
            {
                if (scene->pNode[i].nIdx > -1)
                {
                    meshes[scene->pNode[i].nIdx].name = nodeObjects[i].name;

                    if (scene->pNode[i].nIdxMaterial > -1 && scene->pNode[i].nIdxMaterial < materials.Count)
                    {
                        MeshRenderer renderer = nodeObjects[i].AddComponent<MeshRenderer>();
                        renderer.sharedMaterial = materials[scene->pNode[i].nIdxMaterial];
                    }
                    
                    MeshFilter filter = nodeObjects[i].AddComponent<MeshFilter>();
                    filter.sharedMesh = meshes[scene->pNode[i].nIdx];

                    if (scene->nNumCamera > 0)
                    {
                        for (int j = 0; j < scene->nNumCamera; j++)
                        {
                            if (scene->pCamera[j].nIdxTarget == scene->pNode[i].nIdx)
                            {
                                AddCamera(nodeObjects[i], scene->pCamera[j]);
                            }
                        }
                    }

                    if (scene->nNumLight > 0)
                    {
                        for (int j = 0; j < scene->nNumLight; j++)
                        {
                            if (scene->pLight[j].nIdxTarget == scene->pNode[i].nIdx)
                            {
                                AddLight(nodeObjects[i], scene->pLight[j]);
                            }
                        }
                    }
                }
            }

            if (scene->nNumFrame > 0 && scene->nFPS > 0)
            {
                AnimationClip clip = GenerateAnimation(scene, nodeObjects);
                clip.name = podName;

                root.AddComponent<Animation>().clip = clip;

                ctx.AddObjectToAsset("POD_ANIMATION", clip);
            }
        }

        root.transform.localScale = new Vector3(-1f, 1f, 1f);
        PODNative.destroy_pod(scene);
    }

    private AnimationClip GenerateAnimation(SPODScene* scene, List<GameObject> objects)
    {
        AnimationClip clip = new AnimationClip();
        clip.legacy = true;

        for (int i = 0; i < scene->nNumNode; i++)
        {
            SPODNode node = scene->pNode[i];

            Keyframe[][] transformAnimations = new Keyframe[10][];

            /*
                0: pos.x
                1: pos.y
                2: pos.z

                3: rot.x
                4: rot.y
                5: rot.z
                6: rot.w

                7: scl.x
                8: scl.y
                9: scl.z
            */

            for (int j = 0; j < transformAnimations.Length; j++)
            {
                transformAnimations[j] = new Keyframe[scene->nNumFrame];
            }

            bool hasPosition = false,
            hasRotation = false,
            hasScale = false;

            if ((node.nAnimFlags & (uint)EPODAnimationData.ePODHasMatrixAni) != 0)
            {
                hasPosition = true;
                hasRotation = true;
                hasScale = true;

                for (uint j = 0; j < scene->nNumFrame; j++)
                {
                    Matrix4x4 matrix = PODConvert.ToUnityMatrix(node.pfAnimMatrix, PODTools.GetIndex(j, node.pnAnimMatrixIdx, 16));

                    Vector3 position = matrix.GetColumn(3);
                    Quaternion rotation = Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));

                    Vector3 scale = new Vector3(matrix.GetColumn(0).magnitude, matrix.GetColumn(1).magnitude, matrix.GetColumn(2).magnitude);

                    float time = j == 0 ? 0 : (float)j / scene->nFPS;

                    transformAnimations[0][j] = LinearizeFrame(new Keyframe(time, position.x), scene->nNumFrame);
                    transformAnimations[1][j] = LinearizeFrame(new Keyframe(time, position.y), scene->nNumFrame);
                    transformAnimations[2][j] = LinearizeFrame(new Keyframe(time, position.z), scene->nNumFrame);

                    transformAnimations[3][j] = LinearizeFrame(new Keyframe(time, rotation.x), scene->nNumFrame);
                    transformAnimations[4][j] = LinearizeFrame(new Keyframe(time, rotation.y), scene->nNumFrame);
                    transformAnimations[5][j] = LinearizeFrame(new Keyframe(time, rotation.z), scene->nNumFrame);
                    transformAnimations[6][j] = LinearizeFrame(new Keyframe(time, rotation.w), scene->nNumFrame);

                    transformAnimations[7][j] = LinearizeFrame(new Keyframe(time, scale.x), scene->nNumFrame);
                    transformAnimations[8][j] = LinearizeFrame(new Keyframe(time, scale.y), scene->nNumFrame);
                    transformAnimations[9][j] = LinearizeFrame(new Keyframe(time, scale.z), scene->nNumFrame);
                }
            }
            else
            {
                if ((node.nAnimFlags & (uint)EPODAnimationData.ePODHasPositionAni) != 0)
                {
                    hasPosition = true;

                    for (uint j = 0; j < scene->nNumFrame; j++)
                    {
                        Vector3 position = PODConvert.ToUnityVector3(node.pfAnimPosition, PODTools.GetIndex(j, node.pnAnimPositionIdx, 3));
                        float time = j == 0 ? 0 : (float)j / scene->nFPS;

                        transformAnimations[0][j] = LinearizeFrame(new Keyframe(time, position.x), scene->nNumFrame);
                        transformAnimations[1][j] = LinearizeFrame(new Keyframe(time, position.y), scene->nNumFrame);
                        transformAnimations[2][j] = LinearizeFrame(new Keyframe(time, position.z), scene->nNumFrame);
                    }
                }

                if ((node.nAnimFlags & (uint)EPODAnimationData.ePODHasRotationAni) != 0)
                {
                    hasRotation = true;

                    for (uint j = 0; j < scene->nNumFrame; j++)
                    {
                        Quaternion rotation = PODConvert.ToUnityQuaternion(node.pfAnimRotation, PODTools.GetIndex(j, node.pnAnimRotationIdx, 4));

                        float time = j == 0 ? 0 : (float)j / scene->nFPS;

                        transformAnimations[3][j] = LinearizeFrame(new Keyframe(time, rotation.x), scene->nNumFrame);
                        transformAnimations[4][j] = LinearizeFrame(new Keyframe(time, rotation.y), scene->nNumFrame);
                        transformAnimations[5][j] = LinearizeFrame(new Keyframe(time, rotation.z), scene->nNumFrame);
                        transformAnimations[6][j] = LinearizeFrame(new Keyframe(time, rotation.w), scene->nNumFrame);
                    }
                }

                if ((node.nAnimFlags & (uint)EPODAnimationData.ePODHasScaleAni) != 0)
                {
                    hasScale = true;

                    for (uint j = 0; j < scene->nNumFrame; j++)
                    {
                        Vector3 scale = PODConvert.ToUnityVector3(node.pfAnimScale, PODTools.GetIndex(j, node.pnAnimScaleIdx, 7));
                        float time = j == 0 ? 0 : (float)j / scene->nFPS;

                        transformAnimations[7][j] = LinearizeFrame(new Keyframe(time, scale.x), scene->nNumFrame);
                        transformAnimations[8][j] = LinearizeFrame(new Keyframe(time, scale.y), scene->nNumFrame);
                        transformAnimations[9][j] = LinearizeFrame(new Keyframe(time, scale.z), scene->nNumFrame);

                        // anyone know why scale uses seven floats? because I don't and neither does PowerVR it seems as they only ever use 3
                    }
                }
            }

            string path = GetGameObjectPath(objects[i]);

            if (hasPosition)
            {
                AnimationCurve xPos = new AnimationCurve(transformAnimations[0]);
                AnimationCurve yPos = new AnimationCurve(transformAnimations[1]);
                AnimationCurve zPos = new AnimationCurve(transformAnimations[2]);

                clip.SetCurve(path, typeof(Transform), "localPosition.x", xPos);
                clip.SetCurve(path, typeof(Transform), "localPosition.y", yPos);
                clip.SetCurve(path, typeof(Transform), "localPosition.z", zPos);
            }

            if (hasRotation)
            {
                AnimationCurve xRot = new AnimationCurve(transformAnimations[3]);
                AnimationCurve yRot = new AnimationCurve(transformAnimations[4]);
                AnimationCurve zRot = new AnimationCurve(transformAnimations[5]);
                AnimationCurve wRot = new AnimationCurve(transformAnimations[6]);

                clip.SetCurve(path, typeof(Transform), "localRotation.x", xRot);
                clip.SetCurve(path, typeof(Transform), "localRotation.y", yRot);
                clip.SetCurve(path, typeof(Transform), "localRotation.z", zRot);
                clip.SetCurve(path, typeof(Transform), "localRotation.w", wRot);
            }

            if (hasScale)
            {
                AnimationCurve xScl = new AnimationCurve(transformAnimations[7]);
                AnimationCurve yScl = new AnimationCurve(transformAnimations[8]);
                AnimationCurve zScl = new AnimationCurve(transformAnimations[9]);

                clip.SetCurve(path, typeof(Transform), "localScale.x", xScl);
                clip.SetCurve(path, typeof(Transform), "localScale.y", yScl);
                clip.SetCurve(path, typeof(Transform), "localScale.z", zScl);
            }
        }

        return clip;
    }

    private static MethodInfo setLeft, setRight;

    private Keyframe LinearizeFrame(Keyframe frame, uint count)
    {
        if (setLeft == null)
        {
            setLeft = typeof(AnimationUtility).GetMethod("Internal_SetKeyLeftTangentMode", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }

        if (setRight == null)
        {
            setRight = typeof(AnimationUtility).GetMethod("Internal_SetKeyRightTangentMode", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }

        for (int i = 0; i < count; i++)
        {
            // Dear Unity Technologies, why?
            object[] parameters = new object[2] { frame, AnimationUtility.TangentMode.Linear };

            setLeft.Invoke(null, parameters);

            frame = (Keyframe)parameters[0];
            parameters[0] = frame;

            setRight.Invoke(null, parameters);

            frame = (Keyframe)parameters[0];
        }

        return frame;
    }

    private string GetGameObjectPath(GameObject gameObject)
    {
        string path = gameObject.name;

        while (gameObject.transform.parent.parent != null)
        {
            gameObject = gameObject.transform.parent.gameObject;
            path = gameObject.name + "/" + path;
        }

        return path;
    }

    private void AddCamera(GameObject target, SPODCamera podCamera)
    {
        Camera camera = target.AddComponent<Camera>();

        camera.fieldOfView = podCamera.fFOV;

        camera.farClipPlane = podCamera.fFar;
        camera.nearClipPlane = podCamera.fNear;
    }

    private void AddLight(GameObject target, SPODLight podLight)
    {
        Light light = target.AddComponent<Light>();
        light.color = new Color(podLight.pfColour[0], podLight.pfColour[1], podLight.pfColour[2], 1f);

        switch (podLight.eType)
        {
            case EPODLightType.ePODPoint:
                light.type = LightType.Point;
                break;
                    
            case EPODLightType.ePODDirectional:
                light.type = LightType.Directional;
                break;

            case EPODLightType.ePODSpot:
                light.type = LightType.Spot;
                break;
        }

        // I have no clue how to implement the rest of the values ¯\_(ツ)_/¯
    }

    private void SyncDefaultPosition(GameObject gameObject, SPODNode node)
    {
        if (node.pfAnimMatrix != null)
        {
            Matrix4x4 matrix = PODConvert.ToUnityMatrix(node.pfAnimMatrix, 0);   

            gameObject.transform.localPosition = matrix.GetColumn(3);
            gameObject.transform.localRotation = Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));

            gameObject.transform.localScale = new Vector3(matrix.GetColumn(0).magnitude, matrix.GetColumn(1).magnitude, matrix.GetColumn(2).magnitude);
        }
        else
        {
            if (node.pfAnimPosition != null)
            {
                gameObject.transform.localPosition = PODConvert.ToUnityVector3(node.pfAnimPosition, 0);
            }

            if (node.pfAnimRotation != null)
            {
                gameObject.transform.localRotation = PODConvert.ToUnityQuaternion(node.pfAnimRotation, 0);
            }

            if (node.pfAnimScale != null)
            {
                gameObject.transform.localScale = PODConvert.ToUnityVector3(node.pfAnimScale, 0);
            }
        }
    }
}
