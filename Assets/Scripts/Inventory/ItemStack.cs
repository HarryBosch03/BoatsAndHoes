using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ItemStack
{
    public ItemType type;
    public int amount;

    public InventoryChild inventory;
    public int locX;
    public int locY;

    Dictionary<string, object> metadata = new Dictionary<string, object>();

    int rotation;

    public int Rotation
    {
        get => rotation;
        set
        {
            value %= 4;
            if (value < 0) value = 4 + value;
            rotation = value;
        }
    }

    public Vector2Int Size
    {
        get
        {
            if (Rotation % 2 == 0) return type.size;
            else return new Vector2Int(type.size.y, type.size.x);
        }
    }

    public virtual ItemStack Clone ()
    {
        return new()
        {
            type = type,
            amount = amount
        };
    }

    public static implicit operator bool (ItemStack stack)
    {
        if (stack == null) return false;
        if (!stack.type) return false;
        if (stack.amount <= 0) return false;

        return true;
    }

    public void Drop (Vector3 position)
    {
        Object.Instantiate(type.worldItemPrefab, position, Quaternion.identity);
        type = null;
        amount = 0;
    }

    public static bool CanStack (ItemStack to, ItemStack from)
    {
        if (!from) return true;

        if (!to)
        {
            to.amount = from.amount;
            to.type = from.type;
            to.metadata = new Dictionary<string, object>(from.metadata);

            from.amount = 0;
            from.type = null;

            return true;
        }

        if (to.type != from.type) return false;
        if (to.metadata.Count != 0) return false;
        if (from.metadata.Count != 0) return false;

        to.amount = from.amount;
        from.amount = 0;
        from.type = null;
        return true;
    }
}
