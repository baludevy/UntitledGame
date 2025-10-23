using UnityEngine;

public abstract class BaseTool : MonoBehaviour
{
    public ToolInstance instance;
    public Animator toolAnimator;
    
    public abstract void HandleInput();
    public abstract void UpdateTool();
    
    public void Break()
    {
        PlayerInventory.Instance.RemoveItemByID(instance.id);
        Destroy(gameObject);
    }
}