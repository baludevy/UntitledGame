using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Mineable")]
public class MineableData : ScriptableObject
{
    public string mineableName;
    public string description;
    public ToolType canBeMinedWith;
    public int minDropAmount;
    public int maxDropAmount;
    public float maxHealth;
    public AudioClip sound;
    public ItemData droppedItem;
    
    public void DropLoot()
    {}
}