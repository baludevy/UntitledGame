using UnityEngine;

public interface IEnemy
{
    string Name { get; }
    string Description { get; }

    Element Element { get; }

    float MaxHealth { get; set; }
    float CurrentHealth { get; set; }

    void TakeDamage(float damage, bool doFlash);
}