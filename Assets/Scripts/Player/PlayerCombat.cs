using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerCombat : MonoBehaviour
{
    public Color critColor = Color.yellow;
    public Color superEffectiveColor = Color.cyan;
    public static PlayerCombat Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public static void DamageEnemy(float baseDamage, bool crit, BaseEnemy enemy, Vector3 hitPoint, Vector3 hitNormal,
        Element element = Element.Normal, bool hitEffect = true)
    {
        var eff = Combat.CalcEffectiveness(element, enemy.Element);
        var color = Combat.GetColor(eff, crit);
        if (crit)
            PrefabManager.Instance.SpawnTextMarker(hitPoint + new Vector3(Random.Range(-0.3f, 0.3f), -0.1f, 0f),
                Quaternion.LookRotation(hitNormal), "CRIT!", color);

        float damage = Combat.CalculateDamage(baseDamage, crit, eff);
        PrefabManager.Instance.SpawnDamageMarker(hitPoint, Quaternion.LookRotation(hitNormal), damage, color);
        if (hitEffect) PrefabManager.Instance.SpawnSparkles(hitPoint, Quaternion.LookRotation(hitNormal), color);

        var enemyRb = enemy.GetRigidbody();
        if (enemyRb != null)
        {
            var dir = (enemy.transform.position - PlayerCamera.Instance.transform.position).normalized;
            enemy.ApplyKnockback(dir, 20f);
        }

        enemy.TakeDamage(damage, color);
    }

    public static void DamageEnemies(float baseDamage, bool crit, List<BaseEnemy> enemies, Vector3 hitPoint,
        Vector3 hitNormal, Element element = Element.Normal, bool hitEffect = true)
    {
        if (enemies == null || enemies.Count == 0) return;

        Dictionary<Color, List<MeshRenderer>> colorGroups = new Dictionary<Color, List<MeshRenderer>>();

        for (int i = 0; i < enemies.Count; i++)
        {
            BaseEnemy enemy = enemies[i];
            HitEffectiveness effectiveness = Combat.CalcEffectiveness(element, enemy.Element);
            Color color = Combat.GetColor(effectiveness, crit);

            if (crit)
                PrefabManager.Instance.SpawnTextMarker(hitPoint + new Vector3(Random.Range(-0.3f, 0.3f), -0.1f, 0f),
                    Quaternion.LookRotation(hitNormal), "CRIT!", color);

            float damage = Combat.CalculateDamage(baseDamage, crit, effectiveness);

            PrefabManager.Instance.SpawnDamageMarker(enemy.transform.position + Vector3.up,
                Quaternion.LookRotation(hitNormal), damage, color);

            if (hitEffect)
                PrefabManager.Instance.SpawnSparkles(enemy.transform.position + Vector3.up,
                    Quaternion.LookRotation(hitNormal), color);

            Rigidbody rb = enemy.GetRigidbody();
            if (rb != null)
            {
                Vector3 dir = (enemy.transform.position - PlayerCamera.Instance.transform.position).normalized;
                enemy.ApplyKnockback(dir, 20f);
            }

            enemy.TakeDamage(damage, color, false);

            MeshRenderer[] renderers = enemy.GetComponentsInChildren<MeshRenderer>();
            if (!colorGroups.ContainsKey(color))
                colorGroups[color] = new List<MeshRenderer>();
            colorGroups[color].AddRange(renderers);
        }

        Instance.StartCoroutine(FlashEnemies(colorGroups));
    }
    
    private static IEnumerator FlashEnemies(Dictionary<Color, List<MeshRenderer>> colorGroups)
    {
        foreach (KeyValuePair<Color, List<MeshRenderer>> group in colorGroups)
        {
            yield return Effects.Flash(group.Value.ToArray(), group.Key);
        }
    }

    public static void DamageMineable(float baseDamage, bool crit, BaseMineable mineable, Vector3 hitPoint,
        Vector3 hitNormal, ToolType type)
    {
        if (!mineable.canBeMined) return;
        float damage = type == mineable.CanBeMinedWith ? baseDamage : 0;
        if (crit) damage *= PlayerStatistics.Instance.Combat.GetCritMultiplier();

        var color = crit ? Color.yellow : Color.white;
        PrefabManager.Instance.SpawnDamageMarker(hitPoint, Quaternion.LookRotation(hitNormal), damage, color);
        PrefabManager.Instance.SpawnSparkles(hitPoint, Quaternion.LookRotation(hitNormal), color);
        mineable.TakeDamage(damage, crit);
    }
}