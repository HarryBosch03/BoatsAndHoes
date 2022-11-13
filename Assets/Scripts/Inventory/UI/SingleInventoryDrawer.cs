using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class SingleInventoryDrawer : MonoBehaviour
{
    [SerializeField] InventoryParent parent;
    [SerializeField] InventoryChildDrawer childDrawerPrefab;
    [SerializeField] Transform instanceContainer;

    HashSet<InventoryChildDrawer> childDrawerInstances = new HashSet<InventoryChildDrawer>();

    public void Rebuild()
    {
        foreach (var instance in childDrawerInstances)
        {
            Destroy(instance.gameObject);
        }

        childDrawerInstances.Clear();

        foreach (var inventoryChild in parent.Children)
        {
            var instance = Instantiate(childDrawerPrefab, instanceContainer);
            instance.Target = inventoryChild;
            childDrawerInstances.Add(instance);
        }
    }
}
