using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PodUnityTest))]
public class PodUnityTestInspector : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("podFile"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("podScene"));

        if (GUILayout.Button("Load"))
        {
            ((PodUnityTest)target).Load();
        }

        if (((PodUnityTest)target).canSave && GUILayout.Button("Save"))
        {
            ((PodUnityTest)target).Save();
        }

        if (GUILayout.Button("Load All"))
        {
            ((PodUnityTest)target).LoadAll();
        }

        if (GUILayout.Button("Save All"))
        {
            ((PodUnityTest)target).SaveAll();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
