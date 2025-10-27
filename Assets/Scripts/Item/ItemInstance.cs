using System;
using UnityEngine;

public class ItemInstance
{
    public readonly ItemData data;
    public int amount;

    public ItemInstance(ItemData data, int count = 1)
    {
        this.data = data;
        amount = count;
    }

    public override bool Equals(object obj)
    {
        if (obj is not ItemInstance other) return false;
        return data == other.data && amount == other.amount;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(data, amount);
    }
}