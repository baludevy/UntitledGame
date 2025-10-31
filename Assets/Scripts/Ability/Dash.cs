using System.Collections;
using UnityEngine;
using EZCameraShake;

[CreateAssetMenu(menuName = "Ability/Dash")]
public class Dash : Ability
{
    public float force = 30f;
    public float duration = 0.15f;
    public float damage;
    public float radius;
    public float recoilForce = 15f;
    public float rayDistance = 5f;
    public float rayThickness = 0.5f;

    private bool dashing;

    public override bool Activate()
    {
        PlayerMovement player = PlayerMovement.Instance;
        Rigidbody rb = player.GetRigidbody();
        rb.velocity = Vector3.zero;
        PerformDash(rb);
        CameraShaker.Instance?.ShakeOnce(2f, 2f, 0.05f, 0.3f);
        return true;
    }

    private void PerformDash(Rigidbody rb)
    {
        dashing = true;
        bool playerGrounded = PlayerMovement.Instance.IsGrounded();
        Vector2 movingDir = PlayerMovement.Instance.GetInputDirection();
        Vector3 targetDir = PlayerCamera.Instance.transform.forward;

        if (movingDir.x > 0)
        {
            targetDir = PlayerCamera.Instance.transform.right;
            if (!playerGrounded) targetDir += Vector3.up * 0.3f;
        }
        else if (movingDir.x < 0)
        {
            targetDir = -PlayerCamera.Instance.transform.right;
            if (!playerGrounded) targetDir += Vector3.up * 0.3f;
        }
        else if (movingDir.y < 0)
        {
            targetDir = -PlayerCamera.Instance.transform.forward;
        }

        targetDir.y = playerGrounded ? 0f : targetDir.y;
        targetDir = targetDir.normalized;
        float adjustedForce = playerGrounded ? force * PlayerMovement.Instance.GetDrag() : force;
        Vector3 dashVelocity = targetDir * adjustedForce;
        rb.velocity = dashVelocity;
    }

    public void OnContact()
    {
        if (!dashing) return;
        PlayerMovement player = PlayerMovement.Instance;
        Rigidbody rb = player.GetRigidbody();
        Vector3 origin = PlayerCamera.Instance.transform.position;
        Vector3 dir = PlayerCamera.Instance.transform.forward;
        Vector3 camForward = dir;

        Vector3 point1 = origin - Vector3.up * 0.5f;
        Vector3 point2 = origin + Vector3.up * 0.5f;

        RaycastHit hit;
        if (Physics.CapsuleCast(point1, point2, rayThickness, dir, out hit, rayDistance))
        {
            Collider[] colliders = Physics.OverlapSphere(hit.point, radius);
            bool hitEnemy = false;

            foreach (Collider collider in colliders)
            {
                IDamageable damageable = GetTarget(collider.transform);
                if (damageable is BaseEnemy enemy)
                {
                    PlayerCombat.DamageEnemy(damage, false, enemy, collider.transform.position + Vector3.up,
                        Vector3.zero, Element.Wind, hitEffect: false);
                    hitEnemy = true;
                }
            }

            if (hitEnemy)
            {
                Vector3 recoilDir = -rb.velocity.normalized;
                if (camForward.y < -0.3f) recoilDir = Vector3.up;
                rb.velocity = recoilDir * recoilForce;
            }
        }

        dashing = false;
    }

    private static IDamageable GetTarget(Transform target)
    {
        while (target != null)
        {
            var m = target.GetComponent<IDamageable>();
            if (m != null) return m;
            target = target.parent;
        }

        return null;
    }

    public override void Cancel()
    {
        dashing = false;
    }
}