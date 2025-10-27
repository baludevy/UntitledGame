using UnityEngine;

public class ToolInstance : ItemInstance
{
    public float currentDurability;

    public ToolInstance(ToolData data, int count = 1) : base(data, count)
    {
        currentDurability = data.maxDurability;
    }

    public void TakeDurability()
    {
        currentDurability -= 1;

        ToolData toolData = (ToolData)data;
        
        float durabilityNormalized = currentDurability / toolData.maxDurability;
        PlayerInventory.Instance.GetActiveHotbarSlot().SetFrameFill(durabilityNormalized);
        
        if (currentDurability <= 0)
        {
            ToolController.Instance.currentTool.Break();
        }
    }
}