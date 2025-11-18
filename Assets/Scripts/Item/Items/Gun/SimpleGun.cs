using UnityEngine;

public class SimpleGun : Gun
{
    [SerializeField] private LineRenderer shotLine;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float shotDuration = 0.05f;

    private float shotTimer;

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
        if (Input.GetMouseButtonDown(0) && GunController.Instance.useTimer <= 0)
        {
            GunController.Instance.AddRecoil(data.recoilAmount);
            AudioManager.Play(data.shootAudio, Vector3.zero, 0.9f, 1.1f, 0.3f, false);
            Shoot();
            GunController.Instance.useTimer = data.cooldown;
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
                    PlayerCombat.DamageEnemy(data.damage, enemy, hit.point, hit.normal, toolType: ToolType.Neutral);
                }

                endPoint = hit.point;
            }
        }


        shotLine.SetPosition(0, muzzle.position);
        shotLine.SetPosition(1, endPoint);
        shotLine.enabled = true;
        shotTimer = shotDuration;
    }

    public override void UpdateGun()
    {
    }
}