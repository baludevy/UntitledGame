using System;
using System.Collections;
using UnityEngine;

public class HeldItemController : MonoBehaviour
{
    public Transform heldItemHolder;
    private GameObject currentItemObject;
    private ItemInstance lastItem;

    private Animator itemRootAnimator;

    public static HeldItemController Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        itemRootAnimator = transform.GetChild(0).GetComponent<Animator>();
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

    public void UpdateHeldItem(ItemInstance item)
    {
        if (item == null)
        {
            lastItem = null;
            ClearHeldItem();
            return;
        }
    
        if (lastItem == item) return;

        if (currentItemObject != null)
            Destroy(currentItemObject);

        lastItem = item;

        if (item.data.heldPrefab != null && heldItemHolder.childCount > 0)
        {
            itemRootAnimator.SetTrigger("Equip");
            
            currentItemObject = Instantiate(item.data.heldPrefab, itemRootAnimator.transform);

            foreach (Transform child in currentItemObject.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("HeldItem");
            }

            if (item.data is ToolData)
            {
                MeleeTool meleeTool = currentItemObject.GetComponent<MeleeTool>();
                meleeTool.instance = (ToolInstance)item;
                ToolController.Instance.SetTool(meleeTool);
            }
        }
    }
}