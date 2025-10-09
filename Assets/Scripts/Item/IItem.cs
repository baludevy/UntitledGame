using UnityEngine;

public interface IItem
{
    string Name { get; }
    Sprite Icon { get; }
    void OnPickup();
    void OnUse();
}