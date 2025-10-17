using UnityEngine.EventSystems;

public class InventorySlot : BaseSlot
{
    public int row;
    public int col;
    public ItemTooltip tooltip;

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (dragData == null) return;

        base.OnEndDrag(eventData);
        var target = GetDragTarget(eventData);

        if (target == null)
        {
            PlayerInventory.Instance.DropItem(dragData.item, !dragData.splitting);
            return;
        }

        switch (target)
        {
            // if the slot we released the mouse button is an inventory slot
            case InventorySlot inv when inv != this:
                // if we're splitting
                if (dragData.splitting)
                {
                    var targetItem = PlayerInventory.Instance.GetItem(inv.row, inv.col);

                    // if the slot has an item
                    if (targetItem != null)
                    {
                        // first try merge
                        if (PlayerInventory.Instance.CanMergeItem(targetItem, dragData.item))
                        {
                            PlayerInventory.Instance.SetItem(dragData.item, inv.row, inv.col);
                        }
                        // if cant merge then return the dragged item to its original state
                        else
                        {
                            PlayerInventory.Instance.AddAmountToItem(item, dragData.item.stackAmount);
                        }
                    }
                    // if the slot is empty then move the new halved instance to the target slot
                    else
                    {
                        PlayerInventory.Instance.SetItem(dragData.item, inv.row, inv.col);
                    }
                }
                // if we arent splitting then simply swap the items
                else
                {
                    PlayerInventory.Instance.SwapItems(dragData.item, this, inv);
                }

                break;

            case ContainerSlot slot:
                if (PlayerUIManager.Instance.openedChest != null)
                {
                    var chest = PlayerUIManager.Instance.openedChest;

                    if (dragData.splitting)
                    {
                        if (chest.GetItem(slot.row, slot.col) == null)
                        {
                            chest.SetItem(dragData.item, slot.row, slot.col);
                        }
                        else
                        {
                            if (PlayerInventory.Instance.CanMergeItem(dragData.item, chest.GetItem(slot.row, slot.col)))
                            {
                                chest.AddAmountToItem(chest.GetItem(slot.row, slot.col), dragData.item.stackAmount);
                            }
                            else
                            {
                                PlayerInventory.Instance.SetItem(chest.GetItem(slot.row, slot.col), row, col);
                                chest.SetItem(dragData.item, slot.row, slot.col);
                            }
                        }
                    }
                    else
                    {
                        if (chest.GetItem(slot.row, slot.col) == null)
                        {
                            chest.SetItem(dragData.item, slot.row, slot.col);
                            PlayerInventory.Instance.RemoveItem(row, col);
                        }
                        else
                        {
                            if (PlayerInventory.Instance.CanMergeItem(dragData.item, chest.GetItem(slot.row, slot.col)))
                            {
                                chest.AddAmountToItem(chest.GetItem(slot.row, slot.col), dragData.item.stackAmount);
                                PlayerInventory.Instance.RemoveItem(row, col);
                            }
                            else
                            {
                                PlayerInventory.Instance.SetItem(chest.GetItem(slot.row, slot.col), row, col);
                                chest.SetItem(dragData.item, slot.row, slot.col);
                            }
                        }
                    }
                }

                break;
        }
    }
}
