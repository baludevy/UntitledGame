using System;
using EZCameraShake;
using UnityEngine;

public class ToolController : MonoBehaviour
{
    public BaseTool currentTool;

    [NonSerialized] public float useTimer;

    public static ToolController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (useTimer > 0)
            useTimer -= Time.deltaTime;
        
        if (currentTool != null)
        {
            currentTool.HandleInput();
        }

        if (currentTool != null)
        {
            currentTool.UpdateTool();
        }
    }

    public void SetTool(BaseTool tool)
    {
        currentTool = tool;
    }
}