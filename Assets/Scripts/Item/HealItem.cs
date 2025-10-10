using UnityEngine;

[CreateAssetMenu(menuName = "Items/Healing Item")]
public class HealItem : ItemData
{
    [SerializeField] int healAmount;

    public override void OnPickup()
    {
        base.OnPickup();
    }

    public override void OnUse()
    {
        base.OnUse();
        Debug.Log($"healing {healAmount}");
        PlayerInventory.Instance.RemoveHeldItem();
        PlayerStatistics.Instance.health += healAmount;
    }
}