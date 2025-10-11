using System;
using UnityEngine;
using UnityEngine.Serialization;

public class DroppedItem : MonoBehaviour
{
    public ItemData itemData;
    
    public ItemInstance itemInstance;
    
    [FormerlySerializedAs("placed")] public bool newItem;

    private void Start()
    {
        if (itemInstance == null && newItem)
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