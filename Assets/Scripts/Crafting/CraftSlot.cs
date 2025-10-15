using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftSlot : BaseSlot
{
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
            if (item != null) Clear();
            return;
        }

        if (target is InventorySlot invSlot)
        {
            PlayerInventory.Instance.SwapWithCraft(this, invSlot);
        }
        else if (target is CraftSlot craftSlot && craftSlot != this)
        {
            if (item?.data is ResourceItem)
            {
                (item, craftSlot.item) = (craftSlot.item, item);
                SetItem(item);
                craftSlot.SetItem(craftSlot.item);
            }
        }
    }
}