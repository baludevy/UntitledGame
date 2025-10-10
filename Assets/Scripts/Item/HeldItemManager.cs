using System;
using UnityEngine;

public class HeldItemManager : MonoBehaviour
{
    public Transform heldItemHolder;
    private GameObject currentItemObject;

    private void Update()
    {
        ItemData item = PlayerInventory.Instance.GetActiveItem();

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

            if (Input.GetMouseButtonDown(0) && item.Type == ItemType.weapon)
            {
                item.OnUse();
            }

            if (Input.GetButtonDown("Use") && item.Type == ItemType.consumable)
            {
                item.OnUse();
            }
        }
    }


    private void UpdateHeldItem(ItemData item)
    {
        if (currentItemObject != null && currentItemObject.name == item.heldPrefab.name)
            return;

        if (currentItemObject != null)
            Destroy(currentItemObject);

        if (item.heldPrefab != null)
        {
            currentItemObject = Instantiate(item.heldPrefab, heldItemHolder);
            currentItemObject.transform.localPosition = Vector3.zero;
            currentItemObject.transform.localRotation = Quaternion.identity;
            currentItemObject.transform.localScale = Vector3.one;
        }
    }
}