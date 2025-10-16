using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemInstance item;
    public RawImage icon;
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

    public virtual void SetItem(ItemInstance newItem)
    {
        item = newItem;
        bool hasItem = item != null && item.data.Icon != null;
        icon.gameObject.SetActive(hasItem);
        icon.texture = hasItem ? item.data.Icon.texture : null;
        stackText.text = hasItem && item.data.Stackable ? item.stackAmount.ToString() : "";
        OnSetItem(newItem);
    }

    protected virtual void OnSetItem(ItemInstance newItem)
    {
    }

    public virtual void Clear()
    {
        SetItem(null);
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null) return;

        ItemInstance draggingItem;

        dragData = new GameObject("drag").AddComponent<DragData>();
        
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if(item.stackAmount < 2 || !item.data.Stackable) return;
            
            int half = Mathf.FloorToInt(item.stackAmount / 2);
            draggingItem = new ItemInstance(item.data, half);
            item.stackAmount -= half;
            stackText.text = item.data.Stackable ? item.stackAmount.ToString() : "";
            dragData.splitting = true;
            if (item.stackAmount <= 0) item = null;
        }
        else
        {
            dragData.splitting = false;
            draggingItem = item;
            item = null;
        }
        
        dragData.transform.SetParent(canvas.transform);
        
        dragData.item = draggingItem;

        RawImage img = dragData.AddComponent<RawImage>();
        img.raycastTarget = false;
        img.texture = draggingItem.data.Icon != null ? draggingItem.data.Icon.texture : null;

        RectTransform rt = dragData.GetComponent<RectTransform>();
        rt.sizeDelta = ((RectTransform)icon.transform).sizeDelta;

        CanvasGroup cg = dragData.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        canvasGroup.blocksRaycasts = false;

        UpdateDragPosition(eventData);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (dragData == null) return;
        UpdateDragPosition(eventData);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (dragData != null) 
            Destroy(dragData.gameObject);
        canvasGroup.blocksRaycasts = true;
    }

    private void UpdateDragPosition(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position,
            canvas.worldCamera, out var pos);
        dragData.GetComponent<RectTransform>().anchoredPosition = pos;
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