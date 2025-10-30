using UnityEngine;

public class PlayerCombatStats : MonoBehaviour
{
    public float critChance = 0.05f;
    public float critMultiplier = 1.2f;

    public float GetCritMultiplier() => critMultiplier;
    public float GetCritChance() => critChance;
}