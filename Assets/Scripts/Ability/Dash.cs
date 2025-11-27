using System.Collections;
using UnityEngine;
using EZCameraShake;

[CreateAssetMenu(menuName = "Ability/Dash")]
public class Dash : Ability
{
    public float force = 30f;
    public float duration = 0.1f;
    public float damage;
    public float radius;
    public float recoilForce = 15f;
    public float rayDistance = 5f;
    public float rayThickness = 0.5f;

    private bool dashing;
    private float dashContactUnlockTime;

    public override bool Activate()
    {
        PlayerMovement playerMovement = PlayerMovement.Instance;
        Rigidbody rigidbody = playerMovement.GetRigidbody();
        rigidbody.velocity = Vector3.zero;
        PerformDash(rigidbody);
        CameraShaker.Instance?.ShakeOnce(2f, 2f, 0.05f, 0.3f);
        return true;
    }

    private void PerformDash(Rigidbody rigidbody)
    {
        PlayerMovement playerMovement = PlayerMovement.Instance;
        bool playerGrounded = playerMovement.IsGrounded();

        dashing = true;

        float groundedForceScale = playerGrounded ? 1.5f : 1f;

        dashContactUnlockTime = Time.time + duration;
        playerMovement.SetMovementLocked(true);

        Vector2 inputDirection = playerMovement.GetInputDirection();
        Transform cameraTransform = PlayerCamera.Instance.transform;
        Vector3 targetDirection = GetDashDirection(inputDirection, cameraTransform, playerGrounded);

        targetDirection = NormalizeDashDirection(targetDirection);
        rigidbody.velocity = targetDirection * (force * groundedForceScale);

        PlayerMovement.Instance.StartCoroutine(DashEndRoutine());
    }

    private IEnumerator DashEndRoutine()
    {
        yield return new WaitForSeconds(duration * 2);
        dashing = false;
        PlayerMovement.Instance.SetMovementLocked(false);
    }

    private Vector3 GetDashDirection(Vector2 inputDirection, Transform cameraTransform, bool playerGrounded)
    {
        Vector3 direction = cameraTransform.forward;

        if (inputDirection.x > 0)
            direction = cameraTransform.right;
        else if (inputDirection.x < 0)
            direction = -cameraTransform.right;
        else if (inputDirection.y > 0)
            direction = cameraTransform.forward;
        else if (inputDirection.y < 0)
            direction = -cameraTransform.forward;

        direction.y = playerGrounded ? 0f : direction.y;
        return direction.normalized;
    }

    private Vector3 NormalizeDashDirection(Vector3 direction)
    {
        Vector3 horizontal = new Vector3(direction.x, 0f, direction.z).normalized;
        float vertical = Mathf.Clamp(direction.y, -1f, 1f);
        Vector3 finalDirection = new Vector3(horizontal.x, vertical, horizontal.z).normalized;
        return finalDirection;
    }

    public void OnContact()
    {
        if (!dashing) return;

        PlayerMovement playerMovement = PlayerMovement.Instance;
        Rigidbody rigidbody = playerMovement.GetRigidbody();

        Transform cameraTransform = PlayerCamera.Instance.transform;
        Vector3 origin = cameraTransform.position;
        Vector3 forwardDirection = cameraTransform.forward;

        Vector3 capsuleStart = origin - Vector3.up * 0.5f;
        Vector3 capsuleEnd = origin + Vector3.up * 0.5f;

        if (Physics.CapsuleCast(capsuleStart, capsuleEnd, rayThickness, forwardDirection, out RaycastHit hit,
                rayDistance))
        {
            bool hitEnemy = ApplyAreaDamage(hit.point);

            if (hitEnemy)
            {
                Vector3 recoilDirection = GetRecoilDirection(rigidbody.velocity, cameraTransform.right);
                recoilDirection.y *= 1.2f;
                rigidbody.velocity = recoilDirection * recoilForce;
            }
        }
    }

    private bool ApplyAreaDamage(Vector3 hitPoint)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPoint, radius);
        bool hitEnemy = false;

        foreach (Collider collider in colliders)
        {
            IDamageable damageable = GetTarget(collider.transform);
            if (damageable is BaseEnemy enemy)
            {
                PlayerCombat.DamageEnemy(damage, 15f, enemy, collider.transform.position + Vector3.up, Vector3.zero,
                    Element.Wind, hitEffect: false);
                hitEnemy = true;
            }
        }

        return hitEnemy;
    }

    private Vector3 GetRecoilDirection(Vector3 velocity, Vector3 forwardDirection)
    {
        Vector3 recoilDirection = -velocity.normalized;
        if (forwardDirection.y < -0.3f) recoilDirection = Vector3.up;
        return recoilDirection;
    }

    private static IDamageable GetTarget(Transform target)
    {
        while (target != null)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null) return damageable;
            target = target.parent;
        }

        return null;
    }

    public override void Cancel()
    {
        dashing = false;
        PlayerMovement.Instance.SetMovementLocked(false);
    }
}