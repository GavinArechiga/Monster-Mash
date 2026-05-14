using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WhichPartType))]
public class PartTypes : Editor
{
    string[] options = new string[]
    {
        "Torso",
        "Head",
        "Arm",
        "Leg",
        "Eye",
        "Mouth",
        "Tail",
        "Wing",
        "Horn",
        "Decor"
    };

    public override void OnInspectorGUI()
    {
        WhichPartType type = (WhichPartType)target;

        int index = System.Array.IndexOf(options, type.type);
        if (index < 0) index = 0;

        index = EditorGUILayout.Popup("Type", index, options);
        type.type = options[index];
    }
}
