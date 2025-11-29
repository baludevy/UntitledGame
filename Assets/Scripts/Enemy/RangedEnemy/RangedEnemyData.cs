using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Test Enemy Data")]
public class RangedEnemyData : EnemyData
{
    [Header("Ranged Attack")] public float attackDistance = 3f;
    public float attackInterval = 1.5f;
    public float attackDamage = 10f;
    public float projectileSpeed = 15f;
    [Range(0f, 1f)] public float accuracy = 1f;
}