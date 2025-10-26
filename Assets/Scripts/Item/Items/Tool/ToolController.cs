using System;
using System.Collections.Generic;
using EZCameraShake;
using UnityEngine;

public class ToolController : MonoBehaviour
{
    public Tool currentTool;

    [NonSerialized] public float useTimer;

    public static ToolController Instance;

    public int maxTools = 6;
    public List<Tool> tools;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        foreach (Tool tool in tools)
        {
            AddTool(tool);
        }
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

    public void AddTool(Tool tool)
    {
        if (tool == null || tools.Contains(tool)) return;

        tools.Add(tool);
        PlayerUIManager.Instance.AddToolToToolbar(tool);
    }

    private void SwitchTool(Tool tool)
    {
        if (tool == null || tool == currentTool) return;
    }

    private void SetTool(Tool tool)
    {
        currentTool = tool;
    }
}