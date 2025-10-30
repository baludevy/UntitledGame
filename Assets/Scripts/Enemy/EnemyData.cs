using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Enemy")]
public class EnemyData : ScriptableObject, IEnemy
{
    [SerializeField] private string enemyName;
    [SerializeField] private string description;
    [SerializeField] private float maxHealth;
    
    public string Name
    {
        get => enemyName;
        set => enemyName = value;
    }

    public string Description
    {
        get => description;
        set => description = value;
    }
    
    public float MaxHealth => maxHealth;
}