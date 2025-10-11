using UnityEngine;

[CreateAssetMenu(menuName = "Items/Rock")]
public class Rock : ItemData
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