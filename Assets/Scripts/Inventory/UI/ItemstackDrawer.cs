using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class ItemstackDrawer : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] Image icon;
    [SerializeField] float cellSize;
    [SerializeField] float cellGap;
    [SerializeField] TMP_Text countText;
    [SerializeField] Color emptyBackgroundTint;
    [SerializeField] Color occupiedBackgroundTint;

    Image background;
    Vector2 dragOffset;
    ItemStack stack;

    static ItemstackDrawer DraggedDrawer { get; set; }
    static Dictionary<ItemStack, ItemstackDrawer> CornerDrawers { get; } = new Dictionary<ItemStack, ItemstackDrawer>();

    public InventoryChildDrawer Parent { get; set; }
    public int Index { get; set; }

    private void Start()
    {
        Parent.Target.InventoryChangeEvent += OnInventoryChange;
        icon.transform.SetParent(Parent.transform);
        icon.transform.SetAsLastSibling();
        OnInventoryChange();

        background = GetComponent<Image>();
    }

    private void OnDisable()
    {
        if (!stack) return; 

        CornerDrawers.Remove(stack);
    }

    private void OnDestroy()
    {
        Parent.Target.InventoryChangeEvent -= OnInventoryChange;
    }


    private void Update()
    {
        if (!stack)
        {
            background.color = emptyBackgroundTint;
            icon.enabled = false;
        }
        else 
        {
            background.color = occupiedBackgroundTint;

            icon.rectTransform.position = (Vector2)transform.position + dragOffset;
            icon.rectTransform.rotation = Quaternion.identity;

            if (stack.Rotation % 2 == 1)
            {
                float height = stack.Size.y * cellSize + (stack.Size.y - 1) * cellGap;
                icon.rectTransform.position += Vector3.down * height;
                icon.rectTransform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            }
        }
    }

    private void OnInventoryChange()
    {
        int columns = Parent.Target.Contents.GetLength(0);
        int i = Index % columns;
        int j = Index / columns;

        icon.sprite = null;
        countText.text = string.Empty;

        stack = Parent.Target.Get(i, j);
        if (stack)
        {
            if (Parent.Target.Get(i - 1, j) != stack && Parent.Target.Get(i, j - 1) != stack)
            {
                SetItemDisplay(stack);

                if (CornerDrawers.ContainsKey(stack))
                {
                    CornerDrawers[stack] = this;
                }
                else
                {
                    CornerDrawers.Add(stack, this);
                }
            }
        }

        icon.enabled = icon.sprite;
    }

    private void SetItemDisplay(ItemStack stack)
    {
        float tWidth = stack.Size.x * cellSize + (stack.Size.x - 1) * cellGap;
        float tHeight = stack.Size.y * cellSize + (stack.Size.y - 1) * cellGap;

        if (stack.amount > 1)
        {
            countText.text = stack.amount.ToString();

            countText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tWidth);
            countText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tHeight);
        }

        icon.sprite = stack.type.icon;

        tWidth = stack.type.size.x * cellSize + (stack.type.size.x - 1) * cellGap;
        tHeight = stack.type.size.y * cellSize + (stack.type.size.y - 1) * cellGap;

        void SetDim(RectTransform.Axis axis)
        {
            float size = axis == RectTransform.Axis.Horizontal ? tWidth : tHeight;
            icon.rectTransform.SetSizeWithCurrentAnchors(axis, size);
        };

        SetDim(RectTransform.Axis.Horizontal);
        SetDim(RectTransform.Axis.Vertical);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!stack) return;

        if (CornerDrawers[stack] == this)
        {
            icon.maskable = false;
            DraggedDrawer = this;
        }
        else
        {
            CornerDrawers[stack].OnBeginDrag(eventData);
            DraggedDrawer = this;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!stack) return;

        if (CornerDrawers[stack] == this)
        {
            dragOffset += eventData.delta;
        }
        else
        {
            CornerDrawers[stack].OnDrag(eventData);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!DraggedDrawer) return;

        int columns = Parent.Target.Contents.GetLength(0);
        int x = Index % columns;
        int y = Index / columns;

        int otherColumns = DraggedDrawer.Parent.Target.Contents.GetLength(0);
        int cornerX = CornerDrawers[DraggedDrawer.stack].Index % otherColumns;
        int cornerY = CornerDrawers[DraggedDrawer.stack].Index / otherColumns;
        int otherX = DraggedDrawer.Index % otherColumns;
        int otherY = DraggedDrawer.Index / otherColumns;

        int xDif = cornerX - otherX;
        int yDif = cornerY - otherY;

        x += xDif;
        y += yDif;

        CornerDrawers[DraggedDrawer.stack].dragOffset = Vector2.zero;
        DraggedDrawer.dragOffset = Vector2.zero;
        dragOffset = Vector2.zero;

        CornerDrawers[DraggedDrawer.stack].icon.maskable = true;
        DraggedDrawer.icon.maskable = true;
        icon.maskable = true;

        x = Mathf.Clamp(x, 0, DraggedDrawer.Parent.Target.Contents.GetLength(0) - DraggedDrawer.stack.Size.x);
        y = Mathf.Clamp(y, 0, DraggedDrawer.Parent.Target.Contents.GetLength(1) - DraggedDrawer.stack.Size.y);

        Parent.Target.TryInsert(DraggedDrawer.stack, x, y);
        DraggedDrawer = null;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!stack) return;

        if (DraggedDrawer)
        {
            Ray ray = Camera.main.ScreenPointToRay(eventData.position);

            DraggedDrawer.stack.Drop(ray.GetPoint(1.0f));
        }

        DraggedDrawer = null;
        dragOffset = Vector2.zero;
    }
}
