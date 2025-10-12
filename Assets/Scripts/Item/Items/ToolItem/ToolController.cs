using System;
using UnityEngine;

public class ToolController : MonoBehaviour
{
    private Tool currentTool;

    public static ToolController Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(currentTool != null)
                UseTool();
        }
    }

    public void SetTool(Tool tool)
    {
        currentTool = tool;
    }

    private void UseTool()
    {
        currentTool.Use();
    }
}