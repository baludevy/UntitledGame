using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerBullet : MonoBehaviour
{
    public Rigidbody rb;
    [SerializeField] private float lifeTime = 5f;

    [SerializeField] private ParticleSystem impactPrefab;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Quaternion rotation = Quaternion.LookRotation(contact.normal);
        bool crit = PlayerStatistics.Instance.RollCrit();

        Vector3 spawnPoint = contact.point + contact.normal * 0.05f;

        /* if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            BaseEnemy enemy = collision.gameObject.GetComponent<BaseEnemy>();
            NavMeshAgent agent = enemy != null ? enemy.GetComponent<NavMeshAgent>() : null;

            if (agent != null && agent.velocity.sqrMagnitude > Math.Pow(0.05f, 2))
                spawnPoint = agent.transform.position + agent.velocity * 0.5f +
                             contact.normal * 0.05f;
            else
                spawnPoint = collision.collider.ClosestPoint(contact.point) + contact.normal * 0.05f;

            float damage = 5f;
            if (crit) damage *= PlayerStatistics.Instance.critMultiplier;

            if (enemy != null) enemy.TakeDamage(damage);

            PrefabManager.Instance.SpawnDamageMarker(spawnPoint, rotation, damage, crit);
        } */

        if (collision.gameObject.GetComponent<IDamageable>() != null)
            PrefabManager.Instance.SpawnSparkles(spawnPoint, rotation, crit);

        ParticleSystem fx = Instantiate(impactPrefab, spawnPoint, rotation);
        Destroy(fx.gameObject, 3f);

        Destroy(gameObject);
    }
}