using UnityEngine;
using EZCameraShake;

public class MeleeWeapon : Weapon
{
    private bool swingingThisFrame;
    private bool usedDuringSwing;
    private bool checkAfterFrame;
    private readonly float swingTrigger = 0.2f;

    [SerializeField] private AudioClip swingAudio;

    private WeaponController weaponController;

    public MeleeData data;

    private void Start()
    {
        weaponController = WeaponController.Instance;
    }

    public override void HandleInput()
    {
        if (weaponController == null)
            weaponController = WeaponController.Instance;

        bool isSwinging = Input.GetMouseButton(0);

        if (isSwinging && weaponController.useTimer <= 0 && !swingingThisFrame)
        {
            float baseLength = 1.2f;
            float speedMultiplier = baseLength / data.cooldown;

            weaponAnimator.speed = speedMultiplier;
            weaponAnimator.Play("Swing", 0, 0f);
            AudioManager.Play(swingAudio, Vector3.zero, 0.8f, 1.2f, 0.3f, false);

            usedDuringSwing = false;
            checkAfterFrame = false;
            weaponController.useTimer = data.cooldown;
            swingingThisFrame = true;
        }

        swingingThisFrame = false;
    }

    public override void UpdateWeapon()
    {
        if (checkAfterFrame && !usedDuringSwing)
        {
            var info = weaponAnimator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("Swing") && info.normalizedTime >= swingTrigger)
            {
                CameraShaker.Instance.ShakeOnce(2f, 2f, 0.15f, 0.2f);
                Use();
                usedDuringSwing = true;
            }
        }

        checkAfterFrame = true;
    }

    private void Use()
    {
        if (Physics.Raycast(PlayerCamera.GetRay(), out RaycastHit hit, 5f))
        {
            bool crit = Combat.RollCrit();

            IDamageable damageable = GetTarget(hit.collider.transform);

            if (damageable is BaseMineable mineable)
                PlayerCombat.DamageMineable(data.damage, mineable, hit.point, hit.normal);
            else if (damageable is BaseEnemy enemy)
            {
                PlayerCombat.DamageEnemy(data.damage, 20f, enemy, hit.point, hit.normal);
            }
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
}