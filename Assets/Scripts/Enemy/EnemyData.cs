using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Enemy")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public string description;
    public float maxHealth;
    public Element element;
}