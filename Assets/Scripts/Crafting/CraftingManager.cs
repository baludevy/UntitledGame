using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    [SerializeField] private int craftingGridRows = 3;
    [SerializeField] private int craftingGridCols = 3;

    public Transform craftSlotsHolder;
    private CraftSlot[,] craftSlots;

    public CraftingManager(CraftSlot[,] craftSlots)
    {
        this.craftSlots = craftSlots;
    }

    public static CraftingManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        craftSlots = new CraftSlot[craftingGridRows, craftingGridCols];

        for (int row = 0; row < craftingGridRows; row++)
        {
            for (int col = 0; col < craftingGridCols; col++)
            {
                craftSlots[row, col] = craftSlotsHolder.transform.GetChild(row).GetChild(col).GetComponent<CraftSlot>();
            }
        }
    }

    public void PlaceItem(CraftSlot slot, ItemInstance item)
    {
        if (slot.item != null)
        {
            PlayerInventory.Instance.AddItem(slot.item);
        }

        slot.SetItem(item);
    }
}