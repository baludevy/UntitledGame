using System;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public ItemData testItem;
    public ItemInstance[,] items;
    public int rows = 4;
    public int columns = 6;

    private PlayerUIManager UIManager;

    private void Start()
    {
        UIManager = PlayerUIManager.Instance;

        items = new ItemInstance[rows, columns];
        items[0, 0] = new ItemInstance(testItem, 64);
    }

    public void Interact()
    {
        UIManager.OpenContainerInventory(items, this);
    }

    public ItemInstance GetItem(int row, int col)
    {
        return (row >= 0 && row < rows && col >= 0 && col < columns) ? items[row, col] : null;
    }

    // this sets the slot's item strictly, without trying to merge, overwriting the existing item
    public void SetItemStrict(ItemInstance item, int row, int col)
    {
        items[row, col] = item;

        UIManager.UpdateContainerSlotUI(row, col);
    }

    public void SetItem(ItemInstance item, int row, int col)
    {
        // the existing item if there is any
        var target = items[row, col];

        if (target == null)
        {
            items[row, col] = item;
        }
        else if (PlayerInventory.Instance.CanMergeItem(item, target))
        {
            AddAmountToItem(target, item.stackAmount);
        }

        UIManager.UpdateContainerSlotUI(row, col);
    }

    public void RemoveItem(int row, int col)
    {
        items[row, col] = null;
        PlayerUIManager.Instance.UpdateContainerSlotUI(row, col);
    }

    public (int, int) GetPositionOfItem(ItemInstance item)
    {
        if (item == null) return (-1, -1);

        for (int i = 0; i < rows; i++)
        for (int j = 0; j < columns; j++)
            if (items[i, j] == item)
                return (i, j);

        return (-1, -1);
    }

    public void SwapItems(ItemInstance newItem, ContainerSlot fromSlot, ContainerSlot toSlot)
    {
        var target = items[toSlot.row, toSlot.col];

        if (PlayerInventory.Instance.CanMergeItem(newItem, target))
        {
            target.stackAmount += newItem.stackAmount;
            items[fromSlot.row, fromSlot.col] = null;
            fromSlot.Clear();
        }
        else
        {
            items[fromSlot.row, fromSlot.col] = target;
            items[toSlot.row, toSlot.col] = newItem;
        }

        UIManager.UpdateContainerSlotUI(fromSlot.row, fromSlot.col);
        UIManager.UpdateContainerSlotUI(toSlot.row, toSlot.col);
    }

    public void AddAmountToItem(ItemInstance item, int amount)
    {
        if (item == null || item.stackAmount >= item.data.MaxStack) return;

        int toAdd = Mathf.Clamp(amount, 0, item.data.MaxStack - item.stackAmount);
        if (toAdd <= 0) return;

        item.stackAmount += toAdd;

        var (row, col) = GetPositionOfItem(item);
        UIManager.UpdateContainerSlotUI(row, col);
    }

    public void SubtractAmountFromItem(ItemInstance item, int amount)
    {
        if (item == null || item.stackAmount <= 0) return;

        int toSubtract = Mathf.Clamp(amount, 0, item.stackAmount);
        item.stackAmount -= toSubtract;

        var (row, col) = GetPositionOfItem(item);
        UIManager.UpdateContainerSlotUI(row, col);
    }
}