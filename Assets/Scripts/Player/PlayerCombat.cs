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

    public static void DamageEnemy(float baseDamage, float knockbackForce, BaseEnemy enemy, Vector3 hitPoint,
        Vector3 hitNormal, Element element = Element.Normal, bool hitEffect = true, bool flingUp = false)
    {
        TypeEffectiveness eff = Combat.CalcEffectiveness(element, enemy.Element);

        bool crit = Combat.RollCrit();

        float damage = Combat.CalculateDamage(baseDamage, crit, eff);

        Effects.SpawnEffects(hitPoint, hitNormal, damage, crit, eff, hitEffect);

        Rigidbody enemyRb = enemy.GetRigidbody();
        if (enemyRb != null)
        {
            Vector3 dir = (enemy.transform.position - PlayerCamera.Instance.transform.position).normalized;
            enemy.ApplyKnockback(dir, knockbackForce);
            if(flingUp)
                enemy.ApplyKnockback(Vector3.up, knockbackForce);
        }

        enemy.TakeDamage(damage);
    }


    public static void DamageMineable(float baseDamage, BaseMineable mineable, Vector3 hitPoint,
        Vector3 hitNormal)
    {
        if (!mineable.canBeMined) return;
        float damage = baseDamage;

        bool crit = Combat.RollCrit();

        if (crit)
            damage *= PlayerStatistics.Instance.Combat.GetCritMultiplier();

        Effects.SpawnEffects(hitPoint, hitNormal, damage, crit, TypeEffectiveness.Normal);

        mineable.TakeDamage(damage, crit);
    }
}