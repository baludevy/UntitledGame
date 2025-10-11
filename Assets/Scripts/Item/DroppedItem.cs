using System;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public ItemData itemData;
    
    public ItemInstance itemInstance;
    
    public bool placed;

    private void Start()
    {
        if (itemInstance == null && placed)
        {
            itemInstance = new ItemInstance(itemData, 1);
        }
    }

    public void Initialize(ItemInstance instance)
    {
        itemInstance = instance;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        PlayerInventory.Instance.AddItem(itemInstance);
        itemInstance.OnPickup();
        Destroy(gameObject);
    }
}