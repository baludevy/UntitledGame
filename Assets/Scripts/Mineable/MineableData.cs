using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Mineable")]
public class MineableData : ScriptableObject, IMineable
{
    [SerializeField] private string mineableName;
    [SerializeField] private string description;
    [SerializeField] private ToolType canBeMinedWith;
    [SerializeField] private int minDropAmount;
    [SerializeField] private int maxDropAmount;
    [SerializeField] private float maxHealth;
    [SerializeField] private int sound = -1;
    [SerializeField] private ResourceItem droppedItem;
    
    public string Name
    {
        get => mineableName;
        set => mineableName = value;
    }

    public string Description
    {
        get => description;
        set => description = value;
    }

    public ToolType CanBeMinedWith => canBeMinedWith;
    public int MinDropAmount => minDropAmount;
    public int MaxDropAmount => maxDropAmount;
    
    public float MaxHealth => maxHealth;
    
    public int Sound => sound;

    public ItemData DroppedItem => droppedItem;

    public void DropLoot()
    {
    }
}