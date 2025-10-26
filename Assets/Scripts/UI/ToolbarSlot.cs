using System;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarSlot : MonoBehaviour
{
    [NonSerialized] public Tool tool;
    public RawImage icon;

    private void Start()
    {
        if (tool == null)
        {
            icon.enabled = false;
        }
    }

    public void SetTool(Tool newTool)
    {
        if (newTool == null)
        {
            icon.enabled = false;
            return;
        }

        Debug.Log(newTool.data.Name);
        
        icon.enabled = true;
        tool = newTool;
        icon.texture = newTool.data.Icon.texture;
    }
}