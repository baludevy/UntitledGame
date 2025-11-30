using System.Collections;
using UnityEngine;

public class HitscanGun : Weapon
{
    [SerializeField] private LineRenderer shotLine;
    [SerializeField] private Transform muzzle;

    private GunInstance gun;
    private float shotTimer;
    private bool reloading;

    private void Awake()
    {
        weaponAnimator = GetComponent<Animator>();
    }

    public override void HandleInput()
    {
        if (gun == null) gun = (GunInstance)instance;
        if (reloading) return;

        if (((Input.GetMouseButton(0) && gun.data.automatic) || Input.GetMouseButtonDown(0)) &&
            WeaponController.Instance.useTimer <= 0)
        {
            if (gun.currentAmmo > 0)
            {
                WeaponController.Instance.AddRecoil(gun.data.recoilAmount);
                AudioManager.Play(gun.data.shootAudio, Vector3.zero, 0.9f, 1.1f, 1f, false);

                Shoot();
                WeaponController.Instance.useTimer = gun.data.cooldown;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }

    public override void UpdateWeapon()
    {
        if (gun == null) return;

        if (shotTimer > 0)
        {
            shotTimer -= Time.deltaTime;
            if (shotTimer <= 0) shotLine.enabled = false;
        }
    }

    private void Shoot()
    {
        gun.currentAmmo--;

        Ray ray = PlayerCamera.GetRay();
        
        int enemyBulletLayer = LayerMask.NameToLayer("EnemyBullet");
        int layerMask = ~ (1 << enemyBulletLayer);

        RaycastHit hit;
        bool didHit = Physics.Raycast(ray, out hit, 100f, layerMask, QueryTriggerInteraction.Ignore);

        if (didHit && !hit.collider.CompareTag("Player") && hit.collider.TryGetComponent(out BaseEnemy enemy))
        {
            PlayerCombat.DamageEnemy(
                gun.data.damage,
                gun.data.knockbackAmount,
                enemy,
                hit.point,
                hit.normal
            );
        }

        Vector3 hitPoint = didHit ? hit.point : ray.origin + ray.direction * 100f;

        Vector3 muzzlePos = muzzle.position;
        Vector3 camToMuzzle = muzzlePos - ray.origin;
        float t = Vector3.Dot(camToMuzzle, ray.direction);
        if (t < 0) t = 0;
        Vector3 muzzleOnRay = ray.origin + ray.direction * t;

        float hitT = Vector3.Dot(hitPoint - ray.origin, ray.direction);
        Vector3 clampedEnd = hitT < t ? muzzleOnRay : hitPoint;

        shotLine.positionCount = 2;
        shotLine.SetPosition(0, muzzlePos);
        shotLine.SetPosition(1, clampedEnd);
        shotLine.enabled = true;

        shotTimer = 0.03f;
    }


    private IEnumerator Reload()
    {
        if (reloading || gun.currentAmmo == gun.data.magSize || (!gun.hasInfiniteMags && gun.totalReserveAmmo <= 0))
            yield break;

        reloading = true;
        WeaponController.Instance.SpinWeapon(gun.data.reloadTime);
        AudioManager.Play(gun.data.reloadAudio, Vector3.zero, 0.9f, 1.1f, 1f, false);

        yield return new WaitForSeconds(gun.data.reloadTime);

        int needed = gun.data.magSize - gun.currentAmmo;
        int bulletsToTransfer = gun.hasInfiniteMags ? needed : Mathf.Min(needed, gun.totalReserveAmmo);

        gun.currentAmmo += bulletsToTransfer;
        if (!gun.hasInfiniteMags)
            gun.totalReserveAmmo -= bulletsToTransfer;

        reloading = false;
    }
}