using UnityEngine;

public class ItemData : ScriptableObject, IItem
{
    [SerializeField] string itemName;
    [SerializeField] string itemDescription;
    [SerializeField] Sprite icon;
    [SerializeField] ItemType itemType;
    [SerializeField] int maxStack = 1;
    [SerializeField] bool stackable = true;
    
    public GameObject floorPrefab;
    public GameObject heldPrefab;
    public string Name => itemName;
    public string Description => itemDescription;
    public Sprite Icon => icon;
    public ItemType Type => itemType;
    public int MaxStack => maxStack;
    public bool Stackable => stackable;
    
    public virtual void OnPickup()
    {
        Debug.Log($"picked up {itemName}");
    }

    public virtual void OnUse() {}
}