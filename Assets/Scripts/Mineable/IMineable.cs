using UnityEngine;

public interface IMineable
{
    string Name { get; set; }
    string Description { get; set; }
    int MinDropAmount { get; }
    int MaxDropAmount { get; }

    float MaxHealth { get; set; }
    float CurrentHealth { get; set; }

    void TakeDamage(float amount, bool crit);

    ItemData DroppedItem { get; }

    AudioClip Sound { get; }

    void DropLoot();
}