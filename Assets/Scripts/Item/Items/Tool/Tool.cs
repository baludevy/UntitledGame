using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    public ToolData data;
    public Animator toolAnimator;
    
    public abstract void HandleInput();
    public abstract void UpdateTool();
}