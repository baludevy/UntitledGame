using System;
using UnityEngine;
using Random = UnityEngine.Random;
using EZCameraShake;

public class MeleeTool : BaseTool
{
    private float useTimer;
    private bool wasSwinging;
    private bool swingingThisFrame;
    private bool usedDuringSwing;
    private bool checkAfterFrame;
    private readonly float swingTrigger = 0.2f;

    private void Start()
    {
        ToolData data = (ToolData)instance.data;
        float durabilityNormalized = instance.currentDurability / data.maxDurability;
        PlayerInventory.Instance.GetActiveHotbarSlot().SetFrameFill(durabilityNormalized);
    }

    public override void HandleInput()
    {
        bool isSwinging = Input.GetMouseButton(0);

        if (isSwinging && useTimer <= 0 && !swingingThisFrame)
        {
            ToolData toolData = (ToolData)instance.data;
            float baseLength = 1f;
            float speedMultiplier = baseLength / toolData.cooldown;
            if (toolData.animate)
            {
                toolAnimator.speed = speedMultiplier;
                toolAnimator.Play("Swing", 0, 0f);
            }

            usedDuringSwing = false;
            checkAfterFrame = false;
            useTimer = toolData.cooldown;
            swingingThisFrame = true;
        }

        wasSwinging = isSwinging;
        swingingThisFrame = false;
    }

    public override void UpdateTool(float deltaTime)
    {
        if (useTimer > 0)
            useTimer -= deltaTime;

        if (checkAfterFrame && !usedDuringSwing)
        {
            var info = toolAnimator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("Swing") && info.normalizedTime >= swingTrigger)
            {
                CameraShaker.Instance.ShakeOnce(2f, 2f, 0.15f, 0.2f);
                Use();
                usedDuringSwing = true;
            }
        }

        checkAfterFrame = true;
    }

    protected virtual void Use()
    {
        ToolData data = (ToolData)instance.data;

        if (Physics.Raycast(PlayerCamera.GetRay(), out RaycastHit hit, 5f))
        {
            bool crit = PlayerStatistics.Instance.RollCrit();

            BaseMineable mineable = GetMineable(hit.collider.transform);
            if (mineable != null && mineable.canBeMined)
            {
                float damage = data.type == mineable.CanBeMinedWith ? data.damage : 0;
                
                if (crit)
                    damage *= PlayerStatistics.Instance.critMultiplier;

                GameObject damageMarker = Instantiate(
                    PrefabManager.Instance.damageMarker,
                    hit.point,
                    Quaternion.LookRotation(hit.normal)
                );

                damageMarker.GetComponent<DamageMarker>().ShowDamage(damage, crit);
                mineable.TakeDamage(damage);
            }

            if (hit.collider.GetComponent<IDamageable>() != null)
            {
                ParticleSystem ps =
                    Instantiate(PrefabManager.Instance.hitEffect, hit.point, Quaternion.LookRotation(hit.normal))
                        .GetComponent<ParticleSystem>();

                if (crit)
                {
                    ps.startColor = Color.yellow;
                }
            }
        }
    }

    private static BaseMineable GetMineable(Transform target)
    {
        while (target != null)
        {
            var m = target.GetComponent<BaseMineable>();
            if (m != null)
                return m;

            target = target.parent;
        }

        return null;
    }
}