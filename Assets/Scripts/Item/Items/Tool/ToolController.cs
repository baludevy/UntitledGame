using EZCameraShake;
using UnityEngine;

public class ToolController : MonoBehaviour
{
    public BaseTool currentTool;

    public static ToolController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (!PlayerInventory.Instance.inventoryOpen && currentTool != null)
        {
            currentTool.HandleInput();
        }

        if (currentTool != null)
        {
            currentTool.UpdateTool(Time.deltaTime);
        }
    }

    public void SetTool(BaseTool tool)
    {
        currentTool = tool;
    }
}