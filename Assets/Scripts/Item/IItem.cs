using UnityEngine;

public interface IItem
{
    string Name { get; }
    string Description { get; }
    Sprite Icon { get; }
    int MaxStack { get; }
    bool Stackable { get; }
    void OnPickup();
    void OnUse();
}