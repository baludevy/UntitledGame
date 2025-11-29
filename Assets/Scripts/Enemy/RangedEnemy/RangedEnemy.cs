using UnityEngine;

public class RangedEnemy : BaseEnemy
{
    public GameObject bulletPrefab;
    public LayerMask sightLayerMask;

    private float attackTimer;
    private Rigidbody playerRb;

    private RangedEnemyData RangedData => (RangedEnemyData)data;

    private float AttackDistance => RangedData.attackDistance;
    private float AttackInterval => RangedData.attackInterval;
    private float AttackDamage => RangedData.attackDamage;
    private float ProjectileSpeed => RangedData.projectileSpeed;
    private float Accuracy => RangedData.accuracy;

    protected override void Start()
    {
        base.Start();
        attackTimer = AttackInterval;
        playerRb = PlayerMovement.Instance.GetRigidbody();
    }

    public override void Tick()
    {
        Transform playerTransform = PlayerMovement.Instance.transform;
        if (playerTransform == null || playerRb == null) return;

        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            return;
        }

        Vector3 distanceToPlayer = playerTransform.position - transform.position;
        distanceToPlayer.y = 0;

        float sqrDistance = distanceToPlayer.sqrMagnitude;
        bool inAttackRange = sqrDistance <= AttackDistance * AttackDistance;
        bool playerVisible = CheckLineOfSight(playerTransform.position);

        transform.rotation = Quaternion.LookRotation(distanceToPlayer.normalized);

        if (inAttackRange && playerVisible)
        {
            StopHorizontalMovement();
            HandleAttack();
        }
        else
        {
            MoveTowardsPlayer(distanceToPlayer);
        }
    }

    private void StopHorizontalMovement()
    {
        Rigidbody rb = GetRigidbody();
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
    }

    private void MoveTowardsPlayer(Vector3 distanceToPlayer)
    {
        if (distanceToPlayer.sqrMagnitude < 0.01f) return;

        Rigidbody rb = GetRigidbody();

        Vector3 targetVelocity = distanceToPlayer.normalized * MoveSpeed;
        Vector3 current = rb.velocity;
        Vector3 change = new Vector3(
            targetVelocity.x - current.x,
            0,
            targetVelocity.z - current.z
        );

        rb.AddForce(change * 6f, ForceMode.Acceleration);
        DetectWall();
    }

    private bool CheckLineOfSight(Vector3 targetPosition)
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = (targetPosition - origin).normalized;
        float distance = Vector3.Distance(origin, targetPosition);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, sightLayerMask))
            return hit.collider.CompareTag("Player");

        return true;
    }

    private void HandleAttack()
    {
        attackTimer -= Time.fixedDeltaTime;

        if (attackTimer <= 0)
        {
            Attack();
            attackTimer = AttackInterval;
        }
    }

    private void Attack()
    {
        if (bulletPrefab == null) return;

        Transform playerTransform = PlayerMovement.Instance.transform;

        Vector3 spawnPos = transform.position
                           + transform.forward * 1f
                           + Vector3.up * 1f;

        Vector3 targetPosition = PredictPlayerPosition(
            playerTransform.position,
            playerRb.velocity
        );

        Vector3 idealDirection = (targetPosition - spawnPos).normalized;
        Vector3 fireDirection = ApplyAccuracySpread(idealDirection, Accuracy);

        GameObject bulletGO = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

        Bullet bulletComp = bulletGO.GetComponent<Bullet>();
        if (bulletComp != null)
            bulletComp.Launch(fireDirection, ProjectileSpeed, AttackDamage);
    }

    private Vector3 PredictPlayerPosition(Vector3 playerPos, Vector3 playerVelocity)
    {
        Vector3 displacement = playerPos - transform.position;
        float dist = displacement.magnitude;
        float timeToHit = dist / ProjectileSpeed;

        Vector3 predictedMovement = playerVelocity * timeToHit;

        return playerPos + predictedMovement;
    }

    private Vector3 ApplyAccuracySpread(Vector3 direction, float accuracy)
    {
        if (accuracy >= 1f) return direction;

        float maxAngle = (1f - accuracy) * 15f;

        Quaternion look = Quaternion.LookRotation(direction);
        Quaternion randomSpread = Quaternion.Euler(
            Random.Range(-maxAngle, maxAngle),
            Random.Range(-maxAngle, maxAngle),
            0
        );

        return (look * randomSpread) * Vector3.forward;
    }
}