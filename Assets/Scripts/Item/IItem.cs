using UnityEngine;

public interface IItem
{
    string Name { get; }
    string Description { get; }
    Sprite Icon { get; }
    ItemType Type { get; }
    void OnPickup();
    void OnUse();
}