using UnityEngine;

[CreateAssetMenu(menuName = "Items/Tool")]
public class ToolItem : ItemData
{
    [Header("Tool properties")]
    public int damage;
    public float cooldown;
    public ToolType type;
    public float maxDurability;
}