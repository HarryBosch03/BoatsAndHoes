using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class InventoryChildDrawer : MonoBehaviour
{
    [SerializeField] TMP_Text title;
    [SerializeField] ItemstackDrawer itemDrawerPrefab;
    [SerializeField] GridLayoutGroup layout;

    HashSet<ItemstackDrawer> drawerInstances = new HashSet<ItemstackDrawer>();

    public InventoryChild Target { get; set; }

    private void Start()
    {
        Rebuild();
    }

    public void Rebuild()
    {
        foreach (var instance in drawerInstances)
        {
            Destroy(instance.gameObject);
        }

        drawerInstances.Clear();

        title.text = Target.DisplayName;

        layout.constraintCount = Target.Contents.GetLength(0);

        for (int i = 0; i < Target.Contents.Length; i++)
        {
            ItemstackDrawer instance = Instantiate(itemDrawerPrefab, layout.transform);
            instance.Parent = this;
            instance.Index = i;
            drawerInstances.Add(instance);
        }
    }
}
