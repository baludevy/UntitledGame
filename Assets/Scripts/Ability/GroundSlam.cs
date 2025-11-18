using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;


[CreateAssetMenu(menuName = "Ability/Ground Slam")]
public class GroundSlam : Ability
{
    public float minDamage = 50f;
    public float maxDamage = 100f;
    public float slamRadius = 25f;
    public float force = 25f;
    public float slowDuration = 0.4f;
    public float slowMultiplier = 0.2f;
    public float minHeight = 10f;
    public float maxHeight = 50f;

    private float startHeight;
    private bool slamming;

    public GameObject groundSlamFX;

    public override bool Activate()
    {
        PlayerMovement player = PlayerMovement.Instance;
        Rigidbody rb = player.GetRigidbody();

        if (!IsHighEnough(player)) return false;

        slamming = true;

        // cancel the player's velocity
        rb.velocity = Vector3.zero;

        rb.AddForce(Vector3.down * force, ForceMode.VelocityChange);

        startHeight = GetHeight(player);

        CameraShaker.Instance?.ShakeOnce(2f, 2.5f, 0.1f, 0.2f);
        return true;
    }


    public void OnLand()
    {
        if (!slamming) return;
        slamming = false;

        PlayerMovement player = PlayerMovement.Instance;
        Rigidbody rb = player.GetRigidbody();

        rb.velocity = new Vector3(0f, rb.velocity.y * 0.5f, 0f);

        //slow down the player for some time
        player.ChangeSpeed(player.GetSpeed() * slowMultiplier, slowDuration);
        
        Instantiate(groundSlamFX, player.transform.position, player.transform.rotation);

        SlamEnemies(player.transform.position);

        CameraShaker.Instance?.ShakeOnce(7f, 3f, 0.1f, 0.5f);
    }

    private void SlamEnemies(Vector3 pos)
    {
        PlayerMovement player = PlayerMovement.Instance;

        // make a flat box collider to check for collisions with surrounding enemies
        Collider[] colliders = Physics.OverlapBox(pos, new Vector3(slamRadius / 2f, 3.5f, slamRadius / 2f));

        float currentHeight = GetHeight(player);

        float fallHeight = startHeight - currentHeight;
        float damage = Mathf.Lerp(minDamage, maxDamage, Mathf.InverseLerp(0f, maxHeight, fallHeight));
            
        foreach (Collider collider in colliders)
        {
            IDamageable damageable = GetTarget(collider.transform);
            if (damageable is BaseEnemy enemy)
                PlayerCombat.DamageEnemy(damage, enemy, collider.transform.position + Vector3.up, Vector3.zero,
                    Element.Ground, hitEffect: false);
        }
    }

    private static IDamageable GetTarget(Transform target)
    {
        while (target != null)
        {
            var m = target.GetComponent<IDamageable>();
            if (m != null)
                return m;

            target = target.parent;
        }

        return null;
    }

    private bool IsHighEnough(PlayerMovement player)
    {
        // shoot a raycast down to determine the height
        return !Physics.Raycast(player.transform.position, Vector3.down, out var hit, 100f) ||
               hit.distance >= minHeight;
    }

    private float GetHeight(PlayerMovement player)
    {
        Physics.Raycast(player.transform.position, Vector3.down, out var hit, 100f);

        return hit.distance;
    }

    public override void Cancel()
    {
        slamming = false;
    }
}