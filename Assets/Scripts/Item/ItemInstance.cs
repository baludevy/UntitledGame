using System;
using UnityEngine;

public class ItemInstance
{
    public readonly ItemData data;
    public int amount;
    public Guid id;

    protected ItemInstance(ItemData data, int count = 1)
    {
        this.data = data;
        amount = count;
        id = Guid.NewGuid();
    }
}