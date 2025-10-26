using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarSlot : MonoBehaviour
{
    private Tool tool;
    public RawImage icon;
    public TMP_Text indexText;
    [SerializeField] private Material outlineMaterial;

    private void OnEnable()
    {
        SetSlotState(tool != null);
    }

    public void SetTool(Tool newTool)
    {
        if (newTool == null)
        {
            GetComponent<Image>().enabled = false;
            indexText.gameObject.SetActive(false);
            icon.enabled = false;
            return;
        }

        SetSlotState(true);
        tool = newTool;
        icon.texture = newTool.data.Icon.texture;
    }

    public void SetActiveSlot(bool active)
    {
        icon.material = active ? outlineMaterial : null;
    }

    private void SetSlotState(bool state)
    {
        GetComponent<Image>().enabled = state;
        indexText.gameObject.SetActive(state);
        icon.enabled = state;
    }

    public Tool GetTool()
    {
        return tool;
    }
}