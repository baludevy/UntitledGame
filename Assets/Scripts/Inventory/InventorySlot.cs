using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : BaseSlot
{
    public int row;
    public int col;
    public ItemTooltip tooltip;

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        var results = new List<RaycastResult>();
        var gr = canvas.GetComponent<GraphicRaycaster>();
        gr.Raycast(eventData, results);

        BaseSlot target = null;
        foreach (var r in results)
        {
            target = r.gameObject.GetComponent<BaseSlot>();
            if (target != null) break;
        }

        if (target == null)
        {
            PlayerInventory.Instance.DropItem(item);
            return;
        }

        if (target is InventorySlot invSlot && invSlot != this)
        {
            PlayerInventory.Instance.SwapSlots(this, invSlot);
        }
        else if (target is CraftSlot craftSlot)
        {
            if (item?.data is ResourceItem)
            {
                CraftingManager.Instance.PlaceItem(craftSlot, item);
                PlayerInventory.Instance.RemoveItem(row, col);
                Clear();
            }
        }
    }
}