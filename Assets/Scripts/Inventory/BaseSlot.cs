using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemInstance item;
    public RawImage icon;
    public TMP_Text stackText;
    public bool isActive;
    private GameObject dragIcon;
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
        dragIcon = new GameObject("drag");
        dragIcon.transform.SetParent(canvas.transform, false);
        var img = dragIcon.AddComponent<RawImage>();
        img.raycastTarget = false;
        img.texture = item.data.Icon != null ? item.data.Icon.texture : null;
        var rt = dragIcon.GetComponent<RectTransform>();
        rt.sizeDelta = ((RectTransform)icon.transform).sizeDelta;
        var cg = dragIcon.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        canvasGroup.blocksRaycasts = false;
        UpdateDragPosition(eventData);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (dragIcon == null) return;
        UpdateDragPosition(eventData);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null) Destroy(dragIcon);
        canvasGroup.blocksRaycasts = true;
    }

    private void UpdateDragPosition(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position,
            canvas.worldCamera, out var pos);
        dragIcon.GetComponent<RectTransform>().anchoredPosition = pos;
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