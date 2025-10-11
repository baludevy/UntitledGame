using UnityEngine;

[CreateAssetMenu(menuName = "Items/Pickaxe")]
public class Pickaxe : ItemData
{
    [SerializeField] int damage;

    public override void OnPickup()
    {
        base.OnPickup();
    }

    public override void OnUse()
    {
        base.OnUse();
        Debug.Log("TestWeapon");
    }
}