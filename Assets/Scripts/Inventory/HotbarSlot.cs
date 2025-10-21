using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : BaseSlot
{
    public Image frame;

    protected override void Awake()
    {
        Clear();
    }

    public void SetActive(bool active)
    {
        isActive = active;
        frame.color = new Color(frame.color.r, frame.color.g, frame.color.b, active ? 1f : 0.2f);
    }

    public void SetFrameFill(float fill)
    {
        frame.fillAmount = fill;
    }

    protected override void OnSetItem(ItemInstance newItem)
    {
        if (newItem is ToolInstance tool && newItem.data is ToolItem toolData)
            SetFrameFill(tool.currentDurability / toolData.maxDurability);
        else
            SetFrameFill(1);
    }
}