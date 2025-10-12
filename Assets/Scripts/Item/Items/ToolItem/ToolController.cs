using System.Collections;
using UnityEngine;

public class ToolController : MonoBehaviour
{
    public Transform heldItemHolder;
    public Animator heldItemAnimator;
    private GameObject currentItemObject;
    private ItemData lastItemData;
    private Tool currentTool;

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
            currentTool = null;
            lastItemData = null;
        }
    }
    
    private void UpdateHeldItem(ItemInstance item)
    {
        if (lastItemData == item.data) return;

        if (currentItemObject != null)
            Destroy(currentItemObject);

        lastItemData = item.data;

        if (item.data.heldPrefab != null && heldItemHolder.childCount > 0)
        {
            currentItemObject = Instantiate(item.data.heldPrefab, heldItemHolder.GetChild(0));
            currentItemObject.layer = LayerMask.NameToLayer("HeldItem");
            currentItemObject.transform.localPosition = Vector3.zero;
            currentItemObject.transform.localRotation = Quaternion.identity;
            currentItemObject.transform.localScale = Vector3.one;

            currentTool = currentItemObject?.GetComponent<Tool>();
        }
    }
}