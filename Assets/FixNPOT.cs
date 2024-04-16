using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FixNPOT : Editor
{
    [MenuItem("NPOT/Fix")]
    public static void Fix()
    {
        foreach (string path in Directory.GetFiles(Application.dataPath + "/Resources"))
        {
            if (path.EndsWith(".meta"))
            {
                File.WriteAllText(path, File.ReadAllText(path).Replace("nPOTScale: 1", "nPOTScale: 0"));
            }
        }
    }
}
