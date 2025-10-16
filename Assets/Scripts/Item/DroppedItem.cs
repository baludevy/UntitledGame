using System;
using UnityEngine;
using UnityEngine.Serialization;

public class DroppedItem : MonoBehaviour
{
    public ItemData itemData;
    public ItemInstance itemInstance;

    private Collider playerCollider;
    private float ignoreUntil;

    private void Awake()
    {
        if (itemData is ToolItem item)
            itemInstance = new ToolInstance(item);
        else 
            itemInstance = new ItemInstance(itemData);
    }
    
    public void Initialize(ItemInstance instance, Collider dropperCollider = null)
    {
        itemInstance = instance;

        playerCollider = dropperCollider;
        ignoreUntil = Time.time + 0.5f;

        if (playerCollider != null)
            Physics.IgnoreCollision(GetComponent<Collider>(), playerCollider, true);
    }

    private void Update()
    {
        if (playerCollider != null && Time.time >= ignoreUntil)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), playerCollider, false);
            playerCollider = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time < ignoreUntil) return;

        PickupItem();
    }

    private void PickupItem()
    {
        PlayerInventory.Instance.AddItem(itemInstance);
        itemInstance.OnPickup();
        Destroy(gameObject);
    }
}