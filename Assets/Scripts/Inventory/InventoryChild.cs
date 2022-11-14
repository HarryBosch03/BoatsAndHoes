using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class InventoryChild : MonoBehaviour
{
    [SerializeField] string displayName;
    [SerializeField] Vector2Int size;

    public event System.Action InventoryChangeEvent;

    public InventoryParent Parent { get; set; }
    public string DisplayName => displayName;
    public ItemStack[,] Contents { get; private set; }

    private void Awake()
    {
        Contents = new ItemStack[size.x, size.y];
    }

    public bool TryInsert(ItemStack item, Vector2Int pos) => TryInsert(item, pos.x, pos.y);
    public bool TryInsert (ItemStack item, int x, int y)
    {
        if (x + item.Size.x > Contents.GetLength(0)) return false;
        if (y + item.Size.y > Contents.GetLength(1)) return false;

        for (int i = 0; i < item.Size.x; i++)
        {
            for (int j = 0; j < item.Size.y; j++)
            {
                ItemStack slot = Contents[x + i, y + j];
                if (slot && slot != item) return false;
            }
        }

        if (item.inventory)
        {
            for (int i = 0; i < item.Size.x; i++)
            {
                for (int j = 0; j < item.Size.y; j++)
                {
                    item.inventory.Contents[i + item.locX, j + item.locY] = null;
                }
            }
        }

        item.inventory = this;
        item.locX = x;
        item.locY = y;

        for (int i = 0; i < item.Size.x; i++)
        {
            for (int j = 0; j < item.Size.y; j++)
            {
                Contents[x + i, y + j] = item;
            }
        }

        InventoryChangeEvent?.Invoke();
        return true;
    }

    public bool TryFitItem(ItemStack stack)
    {
        for (int i = 0; i < 4; i++)
        {
            stack.Rotation = i;
            for (int j = 0; j < Contents.GetLength(0) - stack.Size.x + 1; j++)
            {
                for (int k = 0; k < Contents.GetLength(1) - stack.Size.y + 1; k++)
                {
                    if (TryInsert(stack, j, k))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public override string ToString()
    {
        if (Contents == null) Contents = new ItemStack[size.x, size.y];

        StringBuilder builder = new StringBuilder();

        builder.Append("---Inventory Child---\n");
        builder.Append('-', Contents.GetLength(0) + 2);
        builder.Append("\n");

        for (int i = 0; i < Contents.GetLength(1); i++)
        {
            builder.Append("|");
            for (int j = 0; j < Contents.GetLength(0); j++)
            {
                if (Contents[j, i] != null)
                {
                    builder.Append(Contents[j, i].type.displayName[0]);
                }
                else
                {
                    builder.Append(".");
                }
            }
            builder.Append("|\n");
        }
        builder.Append('_', Contents.GetLength(0) + 2);

        return builder.ToString();
    }

    public ItemStack Get (int i)
    {
        return Get(i % Contents.GetLength(0), i / Contents.GetLength(0));
    }

    public ItemStack Get(int i, int j)
    {
        if (i < 0 || i >= Contents.GetLength(0)) return null;
        if (j < 0 || j >= Contents.GetLength(1)) return null;

        return Contents[i, j];
    }
}
