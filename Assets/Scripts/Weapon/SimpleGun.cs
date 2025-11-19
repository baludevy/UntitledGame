using System;
using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;

public class SimpleGun : Weapon
{
    [SerializeField] private LineRenderer shotLine;
    [SerializeField] private Transform muzzle;

    private float shotDuration;

    private float shotTimer;
    
    public GunData data;
    
    private void Update()
    {
        if (shotTimer > 0f)
        {
            shotTimer -= Time.deltaTime;
            if (shotTimer <= 0f) shotLine.enabled = false;
        }
    }

    public override void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && WeaponController.Instance.useTimer <= 0)
        {
            WeaponController.Instance.AddRecoil(data.recoilAmount);
            AudioManager.Play(data.shootAudio, Vector3.zero, 0.9f, 1.1f, 1f, false);
            Shoot();
            WeaponController.Instance.useTimer = data.cooldown;
        }
    }

    private void Shoot()
    {
        Ray ray = PlayerCamera.GetRay();
        RaycastHit hit;
        Vector3 endPoint = ray.origin + ray.direction * 100f;
        if (Physics.Raycast(ray, out hit))
        {
            if (!hit.collider.CompareTag("Player"))
            {
                if (hit.collider.TryGetComponent(out BaseEnemy enemy))
                {
                    PlayerCombat.DamageEnemy(data.damage, data.knockbackAmount, enemy, hit.point, hit.normal);
                }

                endPoint = hit.point;
            }
        }

        shotLine.SetPosition(0, muzzle.position);
        shotLine.SetPosition(1, endPoint);
        shotLine.enabled = true;
        StartCoroutine(DisableShotLineNextFrame());
    }

    private IEnumerator DisableShotLineNextFrame()
    {
        yield return null;
        shotLine.enabled = false;
    }

    public override void UpdateWeapon()
    {
        
    }
}