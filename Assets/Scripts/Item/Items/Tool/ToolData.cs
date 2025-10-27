using UnityEngine;

[CreateAssetMenu(menuName = "Items/Tool")]
public class ToolData : ItemData
{
    [Header("Tool properties")]
    public int damage;
    public float cooldown;
    public ToolType type;
    public float maxDurability;
}