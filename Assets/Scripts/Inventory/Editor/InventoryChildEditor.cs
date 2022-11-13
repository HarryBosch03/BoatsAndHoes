using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InventoryChild))]
public sealed class InventoryChildEditor : Editor
{
    Font font;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!font) font = AssetDatabase.LoadAssetAtPath("Assets/Fonts/Files/SourceCodePro-Regular.ttf", typeof(Font)) as Font;

        GUIStyle s = new GUIStyle();
        s.font = font;
        s.normal.textColor = Color.white;

        string display = target.ToString();
        foreach (var line in display.Split('\n'))
        {
            EditorGUILayout.LabelField(line, s);
        }
    }
}
