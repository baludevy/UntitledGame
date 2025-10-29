using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemInstance item;
    public Image icon;
    public TMP_Text stackText;
    public bool isActive;
    public DragData dragData;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        Clear();
    }

    public void SetItem(ItemInstance newItem)
    {
        item = newItem;
        
        // determine if the item has an icon
        bool hasItem = item != null && item.data.Icon != null;
        icon.gameObject.SetActive(hasItem);
        icon.sprite = hasItem ? item.data.Icon : null;
        stackText.text = hasItem && item.data.Stackable ? item.stackAmount.ToString() : "";
        OnSetItem(newItem);
    }

    protected virtual void OnSetItem(ItemInstance newItem)
    {
    }

    public void Clear()
    {
        SetItem(null);
    }
    

    public virtual void OnBeginDrag(PointerEventData e)
    {
        if (item == null) return;
        
        Debug.Log("a");
        
        GameObject drag = new GameObject("drag", typeof(RectTransform));
        dragData = drag.AddComponent<DragData>();
        drag.transform.SetParent(canvas.transform, false);

        bool split = e.button == PointerEventData.InputButton.Right && item.data.Stackable && item.stackAmount > 1;
        if (split)
        {
            int half = item.stackAmount / 2;
            
            // make a new instance with half the amount of the original item
            dragData.item = new ItemInstance(item.data, half);
            
            PlayerInventory.Instance.SubtractAmountFromItem(item, half);
            stackText.text = item.data.Stackable ? item.stackAmount.ToString() : "";
            dragData.splitting = true;
        }
        else
        {
            dragData.item = item;
            item = null;
            dragData.splitting = false;
        }

        var img = drag.AddComponent<RawImage>();
        img.raycastTarget = false;
        img.texture = dragData.item.data.Icon ? dragData.item.data.Icon.texture : null;

        var rt = drag.GetComponent<RectTransform>();
        rt.sizeDelta = ((RectTransform)icon.transform).sizeDelta;

        drag.AddComponent<CanvasGroup>().blocksRaycasts = false;
        canvasGroup.blocksRaycasts = false;

        UpdateDragPosition(e);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (dragData == null) return;
        UpdateDragPosition(eventData);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (dragData != null)
        {
            Destroy(dragData.gameObject);
        }

        canvasGroup.blocksRaycasts = true;
    }

    private void UpdateDragPosition(PointerEventData eventData)
    {
        if (dragData == null) return;
        var rt = dragData.GetComponent<RectTransform>();
        if (rt == null) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position,
            canvas.worldCamera, out var pos);
        rt.anchoredPosition = pos;
    }

    protected BaseSlot GetDragTarget(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        canvas.GetComponent<GraphicRaycaster>().Raycast(eventData, results);
        foreach (var r in results)
        {
            var slot = r.gameObject.GetComponent<BaseSlot>();
            if (slot != null) return slot;
        }

        return null;
    }
}