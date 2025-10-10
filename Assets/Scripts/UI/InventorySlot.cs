using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    public ItemData item;

    public RawImage icon;
    public RawImage frame;
    public bool isActive;

    private GameObject dragIcon;
    private Canvas canvas;

    private void Awake()
    {
        canvas = transform.parent.parent.parent.parent.GetComponent<Canvas>();
        Clear();
    }

    public void SetItem(ItemData newItem)
    {
        item = newItem;

        if (item != null && item.Icon != null)
        {
            icon.gameObject.SetActive(true);
            icon.texture = item.Icon.texture;
        }
        else
        {
            icon.gameObject.SetActive(false);
            icon.texture = null;
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
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon == null) return;
        dragIcon.transform.position = eventData.position;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }
}