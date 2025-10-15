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
            PlayerInventory.Instance.DropItem(item);
            return;
        }

        switch (target)
        {
            case InventorySlot inv when inv != this:
                PlayerInventory.Instance.SwapSlots(this, inv);
                break;
            case CraftSlot craft when item?.data is ResourceItem:
                CraftingManager.Instance.PlaceItem(craft, item);
                PlayerInventory.Instance.RemoveItem(row, col);
                Clear();
                break;
        }
    }
}