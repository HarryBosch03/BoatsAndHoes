using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ItemStack
{
    public ItemType type;
    public int amount;
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
}
