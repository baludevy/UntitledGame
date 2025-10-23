using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseTool : MonoBehaviour
{
    public ToolInstance instance;
    public Animator toolAnimator;

    private void Start()
    {
        ToolItem data = (ToolItem)instance.data;
        float durabilityNormalized = instance.currentDurability / data.maxDurability;
        PlayerInventory.Instance.GetActiveHotbarSlot().SetFrameFill(durabilityNormalized);
    }

    public virtual void Use()
    {
        ToolItem data = (ToolItem)instance.data;

        if (Physics.Raycast(PlayerCamera.GetRay(), out RaycastHit hit, 5f))
        {
            BaseMineable mineable = GetMineable(hit.collider.transform);
            if (mineable != null && mineable.canBeMined)
            {
                int damage = data.type == mineable.CanBeMinedWith ? data.damage : 0;

                GameObject damageMarker = Instantiate(
                    PrefabManager.Instance.damageMarker,
                    hit.point,
                    Quaternion.LookRotation(hit.normal)
                );

                damageMarker.GetComponent<DamageMarker>().ShowDamage(damage);
                mineable.TakeDamage(damage);
            }

            if (hit.collider.GetComponent<IDamageable>() != null)
            {
                Instantiate(PrefabManager.Instance.hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }

    private BaseMineable GetMineable(Transform t)
    {
        while (t != null)
        {
            var m = t.GetComponent<BaseMineable>();
            if (m != null)
                return m;

            t = t.parent;
        }

        return null;
    }

    public void Break()
    {
        PlayerInventory.Instance.RemoveItemByID(instance.id);
        Destroy(gameObject);
    }
}