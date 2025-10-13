using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InventorySlot : MonoBehaviour
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
            stackText.text = newItem.data.Stackable ? newItem.stackAmount.ToString() : "";
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
}