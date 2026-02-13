using UnityEngine;

public class Sword : MeleeTool {
    protected override void Use() {
        Vector3 origin = PlayerCamera.Instance.transform.position;
        Vector3 direction = PlayerCamera.Instance.transform.forward;
        float slashRange = 2f;
        float slashRadius = 3f;

        Collider[] hits = Physics.OverlapSphere(origin + direction * slashRange * 0.5f, slashRadius);
        foreach (var hit in hits) {
            if (hit.TryGetComponent(out IEnemy enemy)) {
                bool crit = Combat.RollCrit();
                float damage = data.damage;

                Vector3 hitPoint = hit.ClosestPoint(origin);
                Vector3 hitNormal = (hitPoint - origin).normalized;

                hitPoint += -hitNormal * 0.25f;
                Quaternion rot = Quaternion.LookRotation(hitNormal);

                PlayerCombat.DamageEnemy(damage, crit, enemy, hitPoint, hitNormal, Element.Normal, true, data.type);

                // enemy.TakeDamage(damage);
            }
        }
    }
}