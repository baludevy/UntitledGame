using System;
using UnityEngine;

public class ItemInstance
{
    public readonly ItemData data;
    public int stackAmount;
    public Guid id;

    public ItemInstance(ItemData data, int count = 1)
    {
        this.data = data;
        stackAmount = count;
        id = Guid.NewGuid();
    }
    
    public void OnUse()
    {
        data.OnUse();
    }

    public void OnPickup()
    {
        data.OnPickup();
    }
}