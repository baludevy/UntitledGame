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

    public void SetItem(ItemInstance newItem)
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

    public void Clear()
    {
        SetItem(null);
    }

    public virtual void OnBeginDrag(PointerEventData e)
    {
        if (item == null) return;

        dragData = new GameObject("drag").AddComponent<DragData>();
        dragData.transform.SetParent(canvas.transform);

        bool split = e.button == PointerEventData.InputButton.Right && item.data.Stackable &&
                              item.stackAmount > 1;

        if (split)
        {
            int half = item.stackAmount / 2;
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

        var img = dragData.gameObject.AddComponent<RawImage>();
        img.raycastTarget = false;
        img.texture = dragData.item.data.Icon ? dragData.item.data.Icon.texture : null;

        var rt = dragData.GetComponent<RectTransform>();
        rt.sizeDelta = ((RectTransform)icon.transform).sizeDelta;

        dragData.gameObject.AddComponent<CanvasGroup>().blocksRaycasts = false;
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