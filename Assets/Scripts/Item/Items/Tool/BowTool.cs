using UnityEngine;

public class BowTool : BaseTool
{
    private void Use()
    {
        Debug.Log("Bow Use");
        
        ItemInstance arrow = PlayerInventory.Instance.GetArrow();
        
        Debug.Log($"found arrow with {arrow.stackAmount}");
    }

    public override void HandleInput()
    {
        
    }

    public override void UpdateTool(float deltaTime)
    {
        
    }
}