using UnityEngine;

[CreateAssetMenu(menuName = "Items/Healing Item")]
public class HealItem : ItemData
{
    [SerializeField] int healAmount;

    public override void OnUse()
    {
        base.OnUse();
        Debug.Log($"healing {healAmount}");
        PlayerStatistics.Instance.health += healAmount;
    }
}