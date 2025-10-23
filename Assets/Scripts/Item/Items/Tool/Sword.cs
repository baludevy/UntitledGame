using UnityEngine;

public class Sword : MeleeTool
{
    protected override void Use()
    {
        ToolData data = (ToolData)instance.data;
        
        Vector3 origin = PlayerCamera.Instance.transform.position;
        Vector3 direction = PlayerCamera.Instance.transform.forward;
        float slashRange = 2f;
        float slashRadius = 3f;

        RaycastHit[] hits = Physics.SphereCastAll(origin, slashRadius, direction, slashRange);
        foreach (var slashHit in hits)
        {
            if (slashHit.collider.TryGetComponent(out BaseEnemy enemy))
            {
                bool crit = PlayerStatistics.Instance.RollCrit();
                float damage = data.damage;
                if (crit)
                    damage *= PlayerStatistics.Instance.critMultiplier;
                
                GameObject damageMarker = Instantiate(
                    PrefabManager.Instance.damageMarker,
                    slashHit.point,
                    Quaternion.LookRotation(slashHit.normal)
                );
                damageMarker.GetComponent<DamageMarker>().ShowDamage(damage, crit);

                ParticleSystem ps = Instantiate(PrefabManager.Instance.hitEffect, slashHit.point, Quaternion.LookRotation(slashHit.normal))
                    .GetComponent<ParticleSystem>();
                if (crit)
                    ps.startColor = Color.yellow;

                enemy.TakeDamage(damage);
            }
        }
    }
}