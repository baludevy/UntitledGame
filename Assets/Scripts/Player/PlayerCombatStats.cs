using UnityEngine;

public class PlayerCombatStats : MonoBehaviour
{
    [SerializeField] private float critChance = 0.05f;
    [SerializeField] private float critMultiplier = 1.2f;

    public bool RollCrit()
    {
        return Random.value <= critChance;
    }

    public float GetCritMultiplier() => critMultiplier;
    public float GetCritChance() => critChance;
}