using UnityEngine;

public class ItemData : ScriptableObject, IItem
{
    [SerializeField] string itemName;
    [SerializeField] Sprite icon;
    
    public string Name => itemName;
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