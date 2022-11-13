using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InventoryParent))]
public sealed class InventoryParentDrawer : Editor
{
    Font font;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        if (!font) font = AssetDatabase.LoadAssetAtPath("Assets/Fonts/Files/SourceCodePro-Regular.ttf", typeof(Font)) as Font;

        GUIStyle s = new GUIStyle();
        s.font = font;
        s.normal.textColor = Color.white;


        InventoryParent iparent = (InventoryParent)target;
        if (iparent.Children == null) return;
        foreach (var child in iparent.Children)
        {
            string text = child.ToString();
            foreach (var line in text.Split('\n'))
            {
                EditorGUILayout.LabelField(line, s);
            }
        }
    }
}
