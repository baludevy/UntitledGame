using UnityEngine;

public interface IMineable
{
    string Name { get; set; }
    string Description { get; set; }
    ToolType CanBeMinedWith { get; }
    int MinDropAmount { get; }
    int MaxDropAmount { get; }
    
    ItemData DroppedItem { get; }
    
    AudioClip Sound { get; }

    void DropLoot();
}