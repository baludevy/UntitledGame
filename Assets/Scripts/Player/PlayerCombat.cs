using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else

            Destroy(gameObject);
    }

    public static void DamageEnemy(float baseDamage, bool crit, BaseEnemy enemy, Vector3 hitPoint,
        Vector3 hitNormal, Element element = Element.Normal, bool hitEffect = true)
    {
        HitEffectiveness eff = Combat.CalcEffectiveness(element, enemy.Element);
        float damage = Combat.CalculateDamage(baseDamage, crit, eff);

        Effects.SpawnEffects(hitPoint, hitNormal, damage, crit, eff, hitEffect);

        Rigidbody enemyRb = enemy.GetRigidbody();
        if (enemyRb != null)
        {
            Vector3 dir = (enemy.transform.position - PlayerCamera.Instance.transform.position).normalized;
            enemy.ApplyKnockback(dir, 20f);
        }

        enemy.TakeDamage(damage, Effects.GetEffectColor(crit, eff));
    }


    public static void DamageMineable(float baseDamage, bool crit, BaseMineable mineable, Vector3 hitPoint,
        Vector3 hitNormal, ToolType type)
    {
        if (!mineable.canBeMined) return;
        float damage = type == mineable.CanBeMinedWith ? baseDamage : 0;

        if (crit)
            damage *= PlayerStatistics.Instance.Combat.GetCritMultiplier();

        Effects.SpawnEffects(hitPoint, hitNormal, damage, crit, HitEffectiveness.Normal);

        mineable.TakeDamage(damage, crit);
    }
}