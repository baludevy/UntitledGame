using System;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public RawImage icon;
    public RawImage frame;
    public bool isActive;

    private void Awake()
    {
        Clear();
    }

    public void SetActive(bool active)
    {
        isActive = active;
        frame.color = new Color(frame.color.r, frame.color.g, frame.color.b, active ? 1f : 0.1f);
    }

    public void SetItem(ItemData item)
    {
        if(item != null)
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
}