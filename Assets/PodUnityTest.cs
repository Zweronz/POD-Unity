using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PodUnityTest : MonoBehaviour
{
    public PodScene podScene;

    public string podFile;

    private GameObject pod;

    public bool canSave
    {
        get
        {
            return pod != null;
        }
    }

    public void LoadAll()
    {
        foreach (string file in Directory.GetFiles(Application.streamingAssetsPath))
        {
            if (file.EndsWith(".meta"))
            {
                continue;
            }

            podFile = Path.GetFileNameWithoutExtension(file);

            Load();
            pod = null;
        }
    }

    public void SaveAll()
    {
        foreach (string file in Directory.GetFiles(Application.streamingAssetsPath))
        {
            if (file.EndsWith(".meta"))
            {
                continue;
            }

            podFile = Path.GetFileNameWithoutExtension(file);
            
            Load();
            Save();
        }
    }

    public void Load()
    {
        if (pod != null)
        {
            DestroyImmediate(pod);
        }

        new PodFile().Read(ref podScene, Application.streamingAssetsPath + "/" + podFile + ".pod");
        pod = PodUnityFactory.LoadPod(podFile, podScene);
    }

    public void Save()
    {
        string path = Application.dataPath + "/SavedPods/" + podFile;

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string meshPath = path + "/Meshes";

        if (!Directory.Exists(meshPath))
        {
            Directory.CreateDirectory(meshPath);
        }

        string materialPath = path + "/Materials";

        if (!Directory.Exists(materialPath))
        {
            Directory.CreateDirectory(materialPath);
        }

        int index = 0;
        foreach (MeshFilter filter in pod.GetComponentsInChildren<MeshFilter>())
        {
            if (filter.sharedMesh != null)
            {
                try
                {
                    AssetDatabase.CreateAsset(filter.sharedMesh, "Assets/SavedPods/" + podFile + "/Meshes/" + "(" + index++ + ") " + filter.name + ".asset");
                } catch{}
            }
        }

        foreach (MeshRenderer renderer in pod.GetComponentsInChildren<MeshRenderer>())
        {
            if (renderer.sharedMaterial != null && !File.Exists(materialPath + "/" + renderer.sharedMaterial.name + ".mat"))
            {
                AssetDatabase.CreateAsset(renderer.sharedMaterial, "Assets/SavedPods/" + podFile + "/Materials/" + renderer.sharedMaterial.name + ".mat");
            }
        }

        if (pod.GetComponent<Animation>() != null)
        {
            AssetDatabase.CreateAsset(pod.GetComponent<Animation>().clip, "Assets/SavedPods/" + podFile + "/" + podFile + ".anim");
        }
        
        PrefabUtility.SaveAsPrefabAsset(pod, path + "/" + podFile + ".prefab");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
