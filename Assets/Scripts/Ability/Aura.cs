using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Aura")]
public class Aura : Ability
{
    public float radius = 5f;
    public float damage = 10f;
    public float duration = 5f;
    public float damageInterval = 0.5f;
    public Element element = Element.Fire;
    public GameObject auraVisualPrefab;

    private Coroutine damageCoroutine;
    private Coroutine visualUpdateCoroutine;
    private PlayerMovement playerMovement;
    private GameObject currentAuraVisual;

    private const float VISUAL_SCALE_RATIO = 4.5f;

    public override bool Activate()
    {
        playerMovement = PlayerMovement.Instance;

        if (playerMovement == null) return false;

        if (damageCoroutine != null)
        {
            playerMovement.StopCoroutine(damageCoroutine);
        }

        if (visualUpdateCoroutine != null)
        {
            playerMovement.StopCoroutine(visualUpdateCoroutine);
        }

        currentAuraVisual = PrefabManager.Instance.SpawnAuraVisual(playerMovement.transform.position);

        // set visual scale based on radius and user ratio
        float scaleFactor = radius / VISUAL_SCALE_RATIO;
        currentAuraVisual.transform.localScale = Vector3.one * scaleFactor;

        damageCoroutine = playerMovement.StartCoroutine(DamageRoutine());
        visualUpdateCoroutine = playerMovement.StartCoroutine(VisualUpdateRoutine());

        return true;
    }

    private Vector3 CalculateDamageOrigin()
    {
        Transform playerTransform = playerMovement.transform;
        Vector3 origin = playerTransform.position;
        Vector3 damageOrigin = origin;

        const float rayDistance = 100f;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayDistance))
        {
            damageOrigin = hit.point;
        }
        else
        {
            damageOrigin = origin + Vector3.down * 0.5f;
        }

        return damageOrigin;
    }

    private IEnumerator VisualUpdateRoutine()
    {
        while (true)
        {
            if (currentAuraVisual != null)
            {
                Vector3 damageOrigin = CalculateDamageOrigin();
                currentAuraVisual.transform.position = damageOrigin + Vector3.up * 0.01f;
            }

            yield return null;
        }
    }

    private IEnumerator DamageRoutine()
    {
        float startTime = Time.time;
        WaitForSeconds intervalWait = new WaitForSeconds(damageInterval);

        while (Time.time < startTime + duration)
        {
            ApplyAuraDamage();
            yield return intervalWait;
        }

        CancelVisual();
        damageCoroutine = null;
    }

    private void ApplyAuraDamage()
    {
        Vector3 damageOrigin = CalculateDamageOrigin();

        Collider[] colliders = Physics.OverlapSphere(damageOrigin, radius);

        foreach (Collider collider in colliders)
        {
            IDamageable damageable = GetTarget(collider.transform);

            if (damageable is BaseEnemy enemy)
            {
                PlayerCombat.DamageEnemy(
                    damage,
                    0f,
                    enemy,
                    collider.transform.position + Vector3.up * 1f,
                    Vector3.zero,
                    element,
                    hitEffect: true
                );
            }
        }
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

    private void CancelVisual()
    {
        if (currentAuraVisual != null && PrefabManager.Instance != null)
        {
            PrefabManager.Instance.ReturnAuraVisual(currentAuraVisual);
        }

        currentAuraVisual = null;
    }

    public override void Cancel()
    {
        if (damageCoroutine != null && playerMovement != null)
        {
            playerMovement.StopCoroutine(damageCoroutine);
        }

        if (visualUpdateCoroutine != null && playerMovement != null)
        {
            playerMovement.StopCoroutine(visualUpdateCoroutine);
        }

        CancelVisual();
        damageCoroutine = null;
        visualUpdateCoroutine = null;
    }
}