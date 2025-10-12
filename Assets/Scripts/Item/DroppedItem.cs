using System;
using UnityEngine;
using UnityEngine.Serialization;

public class DroppedItem : MonoBehaviour
{
    public ItemData itemData;
    
    public ItemInstance itemInstance;
    
    private void Awake()
    {
        if (itemInstance == null && itemData != null)
            itemInstance = new ItemInstance(itemData);
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