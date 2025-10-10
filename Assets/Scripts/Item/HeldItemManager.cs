using System;
using UnityEngine;

public class HeldItemManager : MonoBehaviour
{
    public Transform heldItemHolder;
    private GameObject currentItemObject;

    private void Update()
    {
        ItemInstance item = PlayerInventory.Instance.GetActiveItem();

        if (item == null)
        {
            if (currentItemObject != null)
            {
                Destroy(currentItemObject);
                currentItemObject = null;
            }
        }
        else
        {
            UpdateHeldItem(item);

            if (Input.GetMouseButtonDown(0) && item.data.Type == ItemType.weapon)
            {
                item.OnUse();
            }

            if (Input.GetButtonDown("Use") && item.data.Type == ItemType.consumable)
            {
                PlayerInventory.Instance.UseActiveItem(1);
            }
        }
    }


    private void UpdateHeldItem(ItemInstance item)
    {
        if (currentItemObject != null && currentItemObject.name.Contains(item.data.heldPrefab.name))
            return;

        if (currentItemObject != null)
            Destroy(currentItemObject);

        if (item.data.heldPrefab != null)
        {
            currentItemObject = Instantiate(item.data.heldPrefab, heldItemHolder);
            currentItemObject.layer = LayerMask.NameToLayer("HeldItem");
            currentItemObject.transform.localPosition = Vector3.zero;
            currentItemObject.transform.localRotation = Quaternion.identity;
            currentItemObject.transform.localScale = Vector3.one;
        }
    }
}