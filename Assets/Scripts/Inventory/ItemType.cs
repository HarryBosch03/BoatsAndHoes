using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item Type")]
public sealed class ItemType : ScriptableObject
{
    public string displayName;
    public Vector2Int size;
    public GameObject worldItemPrefab;
    public Sprite icon;
}
