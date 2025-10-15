using UnityEngine.EventSystems;

public class CraftSlot : BaseSlot
{
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        var target = GetDragTarget(eventData);
        if (target == null)
        {
            if (item != null)
            {
                PlayerInventory.Instance.DropItem(item);
                Clear();
            }

            return;
        }

        switch (target)
        {
            case InventorySlot inv:
                PlayerInventory.Instance.SwapWithCraft(this, inv);
                break;
            case CraftSlot craft when craft != this && item?.data is ResourceItem:
                (item, craft.item) = (craft.item, item);
                SetItem(item);
                craft.SetItem(craft.item);
                break;
        }
    }
}