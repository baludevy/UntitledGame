using UnityEngine;

[CreateAssetMenu(menuName = "Items/Pickaxe")]
public class ToolItem : ItemData
{
    public int damage;
    public float cooldown;

    public override void OnPickup()
    {
        base.OnPickup();
    }

    public override void OnUse()
    {
        base.OnUse();
    }
}