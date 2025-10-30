using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerCombat : MonoBehaviour
{
    public static void DamageEnemy(float baseDamage, bool crit, BaseEnemy enemy, Vector3 hitPoint, Vector3 hitNormal)
    {
        Color color = Color.white;
        if (crit)
        {
            color = Color.yellow;
            PrefabManager.Instance.SpawnCritMarker(hitPoint + new Vector3(0.5f, -0.5f, 0f), Quaternion.LookRotation(hitNormal));
        }

        HitEffectiveness eff = Combat.CalcEffectiveness(Element.Ground, enemy.Element);

        float damage = Combat.CalculateDamage(baseDamage, crit, eff);

        if (eff == HitEffectiveness.SuperEffective) color = Color.cyan;

        PrefabManager.Instance.SpawnDamageMarker(hitPoint, Quaternion.LookRotation(hitNormal), damage, color);
        PrefabManager.Instance.SpawnSparkles(hitPoint, Quaternion.LookRotation(hitNormal), color);

        Rigidbody enemyRb = enemy.GetRigidbody();
        if (enemyRb != null)
        {
            Vector3 dir = (enemy.transform.position - PlayerCamera.Instance.transform.position).normalized;
            enemy.ApplyKnockback(dir, 20f);
        }

        enemy.TakeDamage(damage, color);
    }

    public static void DamageMineable(float baseDamage, bool crit, BaseMineable mineable, Vector3 hitPoint,
        Vector3 hitNormal, ToolType type)
    {
        if (!mineable.canBeMined) return;
        float damage = type == mineable.CanBeMinedWith ? baseDamage : 0;

        if (crit)
            damage *= PlayerStatistics.Instance.Combat.GetCritMultiplier();

        Color color = Color.white;
        if (crit) color = Color.yellow;

        PrefabManager.Instance.SpawnDamageMarker(hitPoint, Quaternion.LookRotation(hitNormal), damage, color);
        PrefabManager.Instance.SpawnSparkles(hitPoint, Quaternion.LookRotation(hitNormal), color);

        mineable.TakeDamage(damage, crit);
    }
}