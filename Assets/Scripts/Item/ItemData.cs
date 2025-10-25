using UnityEngine;

public class ItemData : ScriptableObject, IItem
{
    [Header("Data")]
    [SerializeField] string itemName;
    [SerializeField] string itemDescription;
    [SerializeField] Sprite icon;
    
    public string Name => itemName;
    public string Description => itemDescription;
    public Sprite Icon => icon;
}