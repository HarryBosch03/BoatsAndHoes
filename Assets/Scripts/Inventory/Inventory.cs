using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class Inventory : MonoBehaviour
{
    [SerializeField] Vector2Int size;

    ItemType[,] contents;

    private void Awake()
    {
        contents = new ItemType[size.x, size.y];
    }
}
