using UnityEngine.EventSystems;

public class ContainerSlot : BaseSlot
{
    public int row;
    public int col;

    protected override void SubtractAmount(int amount)
    {
        var chest = PlayerUIManager.Instance.openedChest;
        chest.SubtractAmountFromItem(item, amount);
    }
    
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (dragData == null) return;

        base.OnEndDrag(eventData);
        var target = GetDragTarget(eventData);
        var chest = PlayerUIManager.Instance.openedChest;

        if (target == null)
        {
            PlayerInventory.Instance.DropItem(dragData.item, !dragData.splitting);
            chest.RemoveItem(row, col);
            return;
        }

        switch (target)
        {
            // container to container
            case ContainerSlot slot when slot != this:
                var targetItem = chest.GetItem(slot.row, slot.col);

                if (dragData.splitting)
                {
                    if (targetItem != null)
                    {
                        if (PlayerInventory.Instance.CanMergeItem(dragData.item, targetItem))
                        {
                            chest.SetItem(dragData.item, slot.row, slot.col);
                        }
                        else
                        {
                            chest.AddAmountToItem(item, dragData.item.stackAmount);
                        }
                    }
                    else
                    {
                        chest.SetItem(dragData.item, slot.row, slot.col);
                    }
                }
                else
                {
                    if (targetItem != null)
                    {
                        chest.SwapItems(dragData.item, this, slot);
                    }
                    else
                    {
                        chest.SetItemStrict(dragData.item, slot.row, slot.col);
                        chest.SetItemStrict(null, row, col);
                    }
                }

                break;

            case InventorySlot inv:
                // if we're splitting
                if (dragData.splitting)
                {
                    var targetInvItem = PlayerInventory.Instance.GetItem(inv.row, inv.col);

                    // if the slot has an item
                    if (targetInvItem != null)
                    {
                        // first try merge
                        if (PlayerInventory.Instance.CanMergeItem(targetInvItem, dragData.item))
                        {
                            PlayerInventory.Instance.SetItem(dragData.item, inv.row, inv.col);
                        }
                        // if cant merge then return the dragged item to its original state
                        else
                        {
                            chest.AddAmountToItem(item, dragData.item.stackAmount);
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
                    PlayerInventory.Instance.SetItem(dragData.item, inv.row, inv.col);
                    chest.RemoveItem(row, col);
                    Clear();
                }

                break;
        }
    }
}
