using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Mineable")]
public class EnemyData : ScriptableObject, IEnemy
{
    [SerializeField] private string mineableName;
    [SerializeField] private string description;
    [SerializeField] private float maxHealth;
    
    public string Name
    {
        get => mineableName;
        set => mineableName = value;
    }

    public string Description
    {
        get => description;
        set => description = value;
    }
    
    public float MaxHealth => maxHealth;
}