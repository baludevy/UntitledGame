using UnityEngine;

[CreateAssetMenu(menuName = "Items/Healing Item")]
public class TestWeapon : ItemData
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