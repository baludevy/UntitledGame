using UnityEngine.EventSystems;

public class CraftSlot : BaseSlot
{
    public int row;
    public int col;
    
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
        }
    }
}