using UnityEngine;

public class ItemData : ScriptableObject, IItem
{
    [SerializeField] string itemName;
    [SerializeField] string itemDescription;
    [SerializeField] Sprite icon;
    
    public string Name => itemName;
    public string Description => itemDescription;
    public Sprite Icon => icon;

    public virtual void OnPickup()
    {
        Debug.Log($"picked up {itemName}");
    }

    public virtual void OnUse()
    {
        Debug.Log($"used {itemName}");
    }
}