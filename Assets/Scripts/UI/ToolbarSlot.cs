using System;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarSlot : MonoBehaviour
{
    private Tool tool;
    public RawImage icon;
    [SerializeField] private Material outlineMaterial;

    private void OnEnable()
    {
        icon.enabled = tool != null;
    }

    public void SetTool(Tool newTool)
    {
        if (newTool == null)
        {
            icon.enabled = false;
            return;
        }

        icon.enabled = true;
        tool = newTool;
        icon.texture = newTool.data.Icon.texture;
    }

    public void SetActiveSlot(bool active)
    {
        Debug.Log(active);
        icon.material = active ? outlineMaterial : null;
    }

    public Tool GetTool()
    {
        return tool;
    }
}