using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Enemy")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public string description;
    public float maxHealth;
    public Element element;

    [Header("Ranged Attack")] public float attackDamage = 10f;
    public float projectileSpeed = 15f;
    [Range(0f, 1f)] public float accuracy = 1f;
}