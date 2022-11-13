using UnityEngine;
using UnityEngine.UI;
using TMPro;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class ItemstackDrawer : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] float cellSize;
    [SerializeField] float cellGap;
    [SerializeField] TMP_Text countText;

    public InventoryChildDrawer Parent { get; set; }
    public int Index { get; set; }

    private void Start()
    {
        Parent.Target.InventoryChangeEvent += OnInventoryChange;
        OnInventoryChange();
    }

    private void OnDestroy()
    {
        Parent.Target.InventoryChangeEvent -= OnInventoryChange;
    }

    private void OnInventoryChange()
    {
        int columns = Parent.Target.Contents.GetLength(0);
        int i = Index % columns;
        int j = Index / columns;

        icon.sprite = null;
        countText.text = string.Empty;

        ItemStack stack = Parent.Target.Get(i, j);
        if (stack)
        {
            if (Parent.Target.Get(i - 1, j) != stack && Parent.Target.Get(i, j - 1) != stack)
            {
                SetItemDisplay(stack);
            }
        }

        icon.enabled = icon.sprite;
    }

    private void SetItemDisplay(ItemStack stack)
    {
        if (stack.amount > 1)
        {
            countText.text = stack.amount.ToString();

            float tWidth = stack.Size.x * cellSize + (stack.Size.x - 1) * cellGap;
            float tHeight = stack.Size.y * cellSize + (stack.Size.y - 1) * cellGap;

            countText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tWidth);
            countText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tHeight);
        }

        icon.sprite = stack.type.icon;

        void SetDim(RectTransform.Axis axis, out float size)
        {
            int itemSize = axis == RectTransform.Axis.Horizontal ? stack.type.size.x : stack.type.size.y;

            size = itemSize * cellSize + (itemSize - 1) * cellGap;
            icon.rectTransform.SetSizeWithCurrentAnchors(axis, size);
        };

        SetDim(RectTransform.Axis.Horizontal, out float width);
        SetDim(RectTransform.Axis.Vertical, out float _);

        if (stack.Rotation % 2 == 0)
        {
            icon.rectTransform.anchoredPosition = Vector2.zero;
            icon.rectTransform.rotation = Quaternion.identity;
        }
        else
        {
            icon.rectTransform.anchoredPosition = Vector2.down * width;
            icon.rectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
        }
    }
}
