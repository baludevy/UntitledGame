using UnityEngine;

[CreateAssetMenu(menuName = "Items/Tool")]
public class ToolData : ItemData
{
    [Header("Tool properties")]
    public float damage;
    public float cooldown;
    public ToolType type;
    public float maxDurability;
    public bool animate = true;
}