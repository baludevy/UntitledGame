using UnityEngine;

public interface IItem
{
    string Name { get; }
    string Description { get; }
    Sprite Icon { get; }
}