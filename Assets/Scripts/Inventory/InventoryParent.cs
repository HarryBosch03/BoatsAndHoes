using System;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class InventoryParent : MonoBehaviour
{
    InventoryChild[] children;

    public InventoryChild[] Children => children;

    public bool TryFitItem(ItemStack stack)
    {
        foreach (var child in children)
        {
            if (child.TryFitItem(stack))
            {
                return true;
            }
        }
        return false;
    }

    private void Awake()
    {
        children = GetComponentsInChildren<InventoryChild>();
        foreach (var child in children)
        {
            child.Parent = this;
        }
    }
}
