using System;
using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    public ToolInstance instance;
    public ToolData data;
    public Animator toolAnimator;

    private void Start()
    {
        data = (ToolData)instance.data;
    }

    public abstract void HandleInput();
    public abstract void UpdateTool();

    public void Break()
    {
        
    }
}