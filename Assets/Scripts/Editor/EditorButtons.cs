using UnityEditor;
using UnityEngine;

public class EditorButtons
{
    [MenuItem("Quick Actions/Player Prefab")]
    public static void GotoPlayer ()
    {
        var asset = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Player/Player Avatar.prefab", typeof(GameObject));
        Selection.activeObject = asset;
        AssetDatabase.OpenAsset(asset);
    }
}
