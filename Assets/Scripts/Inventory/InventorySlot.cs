using UnityEngine.EventSystems;

public class InventorySlot : BaseSlot
{
    public int row;
    public int col;
    public ItemTooltip tooltip;

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        var target = GetDragTarget(eventData);

        if (target == null)
        {
            PlayerInventory.Instance.DropItem(dragData.item, !dragData.splitting);
            return;
        }

        switch (target)
        {
            case InventorySlot inv when inv != this:
                var targetItem = PlayerInventory.Instance.GetItem(inv.row, inv.col);

                if (dragData.splitting)
                {
                    if (targetItem == null)
                    {
                        PlayerInventory.Instance.SetItem(dragData.item, inv.row, inv.col);
                    }
                    else
                    {
                        PlayerInventory.Instance.AddItem(dragData.item);
                        SetItem(item);
                    }
                }
                else
                {
                    PlayerInventory.Instance.SwapItems(dragData.item, this, inv);
                }

                break;
        }
    }
}