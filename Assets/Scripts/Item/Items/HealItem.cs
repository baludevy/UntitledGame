using UnityEngine;

[CreateAssetMenu(menuName = "Items/Healing Item")]
public class HealItem : ItemData
{
    [SerializeField] int healAmount;

    public override void OnUse()
    {
        base.OnUse();
        PlayerStatistics.Instance.health += healAmount;
    }
}