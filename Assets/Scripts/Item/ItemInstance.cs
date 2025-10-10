using System;
using UnityEngine;

[SerializeField]
public class ItemInstance
{
    public ItemData data;
    public int stack;
    public Guid id;

    public ItemInstance(ItemData data, int count = 1)
    {
        this.data = data;
        this.stack = count;
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