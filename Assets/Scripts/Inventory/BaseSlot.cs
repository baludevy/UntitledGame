using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemInstance item;
    public RawImage icon;
    public RawImage frame;
    public TMP_Text stackText;
    public bool isActive;
    protected GameObject dragIcon;
    protected Canvas canvas;
    protected CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        Clear();
    }

    public virtual void SetItem(ItemInstance newItem)
    {
        item = newItem;
        if (item != null && item.data.Icon != null)
        {
            icon.gameObject.SetActive(true);
            icon.texture = item.data.Icon.texture;
            stackText.text = item.data.Stackable ? item.stackAmount.ToString() : "";
        }
        else
        {
            Clear();
        }
    }

    public virtual void Clear()
    {
        icon.gameObject.SetActive(false);
        icon.texture = null;
        stackText.text = "";
        item = null;
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

    protected void UpdateDragPosition(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out var pos);
        dragIcon.GetComponent<RectTransform>().anchoredPosition = pos;
    }
}
