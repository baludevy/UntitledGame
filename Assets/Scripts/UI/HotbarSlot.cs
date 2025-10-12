using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : MonoBehaviour
{
    public RawImage icon;
    public RawImage frame;
    public TMP_Text stackText;
    public bool isActive;

    private void Awake()
    {
        Clear();
    }

    public void SetActive(bool active)
    {
        isActive = active;
        frame.color = new Color(frame.color.r, frame.color.g, frame.color.b, active ? 1f : 0.2f);
    }

    public void SetItem(ItemInstance item)
    {
        if (item != null)
        {
            icon.gameObject.SetActive(true);
            icon.texture = item.data.Icon.texture;
            stackText.text = item.data.Stackable ? item.stack.ToString() : "";
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