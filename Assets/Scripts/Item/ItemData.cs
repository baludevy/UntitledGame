using UnityEngine;

public class ItemData : ScriptableObject, IItem
{
    [Header("Data")]
    [SerializeField] string itemName;
    [SerializeField] string itemDescription;
    [SerializeField] Sprite icon;
    
    [Header("Stacking")]
    [SerializeField] int maxStack = 64;
    [SerializeField] bool stackable = true;
    
    public GameObject floorPrefab;
    public GameObject heldPrefab;
    public string Name => itemName;
    public string Description => itemDescription;
    public Sprite Icon => icon;
    public int MaxStack => maxStack;
    public bool Stackable => stackable;
    
    public virtual void OnPickup() {}
    public virtual void OnUse() {}
}