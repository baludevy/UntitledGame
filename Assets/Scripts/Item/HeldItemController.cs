using System.Collections;
using UnityEngine;

public class HeldItemController : MonoBehaviour
{
    public Transform heldItemHolder;
    private GameObject currentItemObject;
    private ItemInstance lastItem;

    private void Update()
    {
        ItemInstance item = PlayerInventory.Instance?.GetActiveItem();

        if (item == null)
        {
            ClearHeldItem();
        }
        else
        {
            UpdateHeldItem(item);
        }
    }

    private void ClearHeldItem()
    {
        if (currentItemObject != null)
        {
            Destroy(currentItemObject);
            currentItemObject = null;
            lastItem = null;
            ToolController.Instance.SetTool(null);
        }
    }
    
    private void UpdateHeldItem(ItemInstance item)
    {
        if (lastItem == item) return;

        if (currentItemObject != null)
            Destroy(currentItemObject);

        lastItem = item;

        if (item.data.heldPrefab != null && heldItemHolder.childCount > 0)
        {
            currentItemObject = Instantiate(item.data.heldPrefab, heldItemHolder.GetChild(0));

            foreach (Transform child in currentItemObject.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("HeldItem");
            }

            if (item.data is ToolItem)
            {
                Tool tool = currentItemObject.GetComponent<Tool>();
                ToolController.Instance.SetTool(tool);
            }
        }
    }
}