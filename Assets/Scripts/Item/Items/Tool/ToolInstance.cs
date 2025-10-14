using UnityEngine;

public class ToolInstance : ItemInstance
{
    public float currentDurability;

    public ToolInstance(ToolItem data, int count = 1) : base(data, count)
    {
        currentDurability = data.maxDurability;
    }

    public void TakeDurability()
    {
        currentDurability -= 1;

        ToolItem toolData = (ToolItem)data;
        
        float durabilityNormalized = currentDurability / toolData.maxDurability;
        PlayerInventory.Instance.GetActiveHotbarSlot().SetFrameFill(durabilityNormalized);
        
        if (currentDurability <= 0)
        {
            ToolController.Instance.currentTool.Break();
        }
    }
}