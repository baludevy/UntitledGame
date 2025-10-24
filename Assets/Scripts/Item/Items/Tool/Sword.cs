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

        Collider[] hits = Physics.OverlapSphere(origin + direction * slashRange * 0.5f, slashRadius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out BaseEnemy enemy))
            {
                bool crit = PlayerStatistics.Instance.RollCrit();
                float damage = data.damage;
                if (crit)
                    damage *= PlayerStatistics.Instance.critMultiplier;

                Vector3 hitPoint = hit.ClosestPoint(origin);
                Vector3 hitNormal = (hitPoint - origin).normalized;

                hitPoint += -hitNormal * 0.25f;
                Quaternion rot = Quaternion.LookRotation(hitNormal);

                PrefabManager.Instance.SpawnDamageMarker(hitPoint, rot, damage, crit);
                PrefabManager.Instance.SpawnSparkles(hitPoint, rot, crit);

                enemy.TakeDamage(damage);
            }
        }
    }
}