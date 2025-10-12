using UnityEngine;

[CreateAssetMenu(menuName = "Items/Tool")]
public class ToolItem : ItemData
{
    public int damage;
    public float cooldown;
    public ToolType type;
}