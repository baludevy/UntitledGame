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
                if (!dragData.splitting)
                    PlayerInventory.Instance.SwapItems(dragData.item, this, inv);
                else
                {
                    if (inv.item != null)
                        PlayerInventory.Instance.DropItem(inv.item);
                    PlayerInventory.Instance.SetItem(dragData.item, inv.row, inv.col);
                }
                
                break;
        }
    }
}