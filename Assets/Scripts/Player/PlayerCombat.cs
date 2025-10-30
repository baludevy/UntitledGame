using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerCombat : MonoBehaviour
{
    public static void DamageEnemy(float baseDamage, bool crit, BaseEnemy enemy, Vector3 hitPoint,
        Vector3 hitNormal, Element element = Element.Normal)
    {
        Color color = Color.white;
        if (crit)
        {
            color = Color.yellow;
            PrefabManager.Instance.SpawnTextMarker(hitPoint + new Vector3(Random.Range(-0.3f, 0.3f), -0.1f, 0f),
                Quaternion.LookRotation(hitNormal), "CRIT!", color);
        }

        HitEffectiveness eff = Combat.CalcEffectiveness(element, enemy.Element);

        if(eff == HitEffectiveness.SuperEffective) color = Color.cyan;
        
        float damage = Combat.CalculateDamage(baseDamage, crit, eff);
        
        Debug.Log(damage);
        Debug.Log(eff.ToString());

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