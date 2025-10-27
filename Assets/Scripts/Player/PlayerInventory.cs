using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private List<ItemInstance> items = new();
    
    private PlayerUIManager uiManager;

    public static PlayerInventory Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Update()
    {
        if(uiManager == null)
            uiManager = PlayerUIManager.Instance;
        
        if (Input.GetButtonDown("Inventory"))
            uiManager.SetInventoryState(!uiManager.GetInventoryState());
    }

    public void AddItem(ItemInstance newItem)
    {
        foreach (ItemInstance item in items)
        {
            if(item.data == newItem.data)
            {
                item.amount += newItem.amount;
                return;
            }
        }
        
        items.Add(newItem);
    }

    public void SubtractFromItem(ItemInstance item, int amount)
    {
        item.amount -= amount;
        if(item.amount <= 0)
            RemoveItem(item);
    }

    public void RemoveItem(ItemInstance item)
    {
        items.Remove(item);
    }

    public List<ItemInstance> GetItems() => items;
}