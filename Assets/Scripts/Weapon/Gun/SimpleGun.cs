using System.Collections;
using UnityEngine;

public class SimpleGun : Weapon
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

        if (Input.GetMouseButtonDown(0) && WeaponController.Instance.useTimer <= 0)
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
            int needed = gun.data.magSize - gun.currentAmmo;
            if (needed > 0 && gun.currentMagCount > 0)
            {
                StartCoroutine(Reload());
            }
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
        Vector3 endPoint = ray.origin + ray.direction * 100f;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (!hit.collider.CompareTag("Player") && hit.collider.TryGetComponent(out BaseEnemy enemy))
            {
                PlayerCombat.DamageEnemy(
                    gun.data.damage,
                    gun.data.knockbackAmount,
                    enemy,
                    hit.point,
                    hit.normal
                );
            }

            endPoint = hit.point;
        }

        shotLine.SetPosition(0, muzzle.position);
        shotLine.SetPosition(1, endPoint);
        shotLine.enabled = true;

        shotTimer = 0.03f;
    }


    private IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(gun.data.reloadTime);

        int needed = gun.data.magSize - gun.currentAmmo;
        if (gun.currentMagCount > 0 && needed > 0)
        {
            gun.currentMagCount--;
            gun.currentAmmo = gun.data.magSize;
        }

        reloading = false;
    }
}