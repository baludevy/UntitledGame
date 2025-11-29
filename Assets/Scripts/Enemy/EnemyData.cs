using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Base Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Meta")] public string enemyName;
    public string description;
    public Element element;

    [Header("Stats")] public float maxHealth = 100f;
    public float moveSpeed = 3f;
}