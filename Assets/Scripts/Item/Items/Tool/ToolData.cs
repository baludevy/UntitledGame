using UnityEngine;

[CreateAssetMenu(menuName = "Tool")]
public class ToolData : ItemData
{
    public GameObject toolPrefab;
    public float damage;
    public float cooldown;
    public ToolType type;
}