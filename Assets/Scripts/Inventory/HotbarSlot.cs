using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : MonoBehaviour
{
    public RawImage icon;
    public Image frame;
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
        frame.transform.localPosition = active ? Vector3.up * 10 : Vector3.zero;
        icon.transform.localPosition = active ? Vector3.up * 10 : Vector3.zero;
    }

    public void SetFrameFill(float fill)
    {
        frame.fillAmount = fill;
    }

    public void SetItem(ItemInstance item)
    {
        if (item != null)
        {
            icon.gameObject.SetActive(true);
            icon.texture = item.data.Icon.texture;
            stackText.text = item.data.Stackable ? item.stackAmount.ToString() : "";

            if (item.data is ToolItem toolData)
            {
                SetFrameFill(ToolController.Instance.currentTool.instance.currentDurability / toolData.maxDurability);
            }
        }
        else
        {
            icon.gameObject.SetActive(false);
            icon.texture = null;
            stackText.text = "";
            SetFrameFill(1);
        }
    }

    public void Clear()
    {
        icon.gameObject.SetActive(false);
        icon.texture = null;
    }
}