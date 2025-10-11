using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    public ItemInstance item;

    public RawImage icon;
    public RawImage frame;
    public TMP_Text stackText;
    public bool isActive;

    private GameObject dragIcon;
    private Canvas canvas;
    
    public ItemTooltip tooltip;

    private void Awake()
    {
        canvas = transform.parent.parent.parent.parent.GetComponent<Canvas>();
        Clear();
    }
    public void SetItem(ItemInstance newItem)
    {
        item = newItem;

        if (item != null && item.data.Icon != null)
        {
            icon.gameObject.SetActive(true);
            icon.texture = item.data.Icon.texture;
            stackText.text = newItem.data.Stackable ? newItem.stack.ToString() : "";
        }
        else
        {
            icon.gameObject.SetActive(false);
            icon.texture = null;
            stackText.text = "";
        }
    }
    
    public void Clear()
    {
        icon.gameObject.SetActive(false);
        icon.texture = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null) return;

        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(canvas.transform);
        dragIcon.transform.SetAsLastSibling();

        icon.gameObject.SetActive(false);
        RawImage image = dragIcon.AddComponent<RawImage>();
        image.texture = icon.texture;
        image.raycastTarget = false;
        dragIcon.GetComponent<RectTransform>().sizeDelta = image.rectTransform.sizeDelta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null) Destroy(dragIcon);

        InventorySlot targetSlot = eventData.pointerEnter?.GetComponent<InventorySlot>();
        if (targetSlot != null && PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.SwapItems(this, targetSlot);
            
            PlayerInventory.Instance.RefreshInventory();
            PlayerInventory.Instance.RefreshHotbar();
        }
        else
        {
            PlayerInventory.Instance.DropItem(item);
        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon == null) return;
        dragIcon.transform.position = eventData.position;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item != null)
            tooltip.Show(item, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Hide();
    }
}