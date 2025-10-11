using UnityEngine;

[CreateAssetMenu(menuName = "Items/Pickaxe")]
public class Pickaxe : ItemData
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