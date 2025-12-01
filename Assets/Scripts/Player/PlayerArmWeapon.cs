using UnityEngine;

public class PlayerArmWeapon : MonoBehaviour
{
    [SerializeField] private float parryRange = 5f;
    [SerializeField] private float parryRadius = 0.35f;

    private void Update()
    {
        if (PlayerMovement.Instance.GetCanLook())
            HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
            Punch();
    }

    private void Punch()
    {
        Ray ray = PlayerCamera.GetRay();
        RaycastHit hit;
        bool didHit = Physics.SphereCast(ray, parryRadius, out hit, parryRange);
        Debug.DrawRay(ray.origin, ray.direction * parryRange, Color.yellow, 0.5f);

        if (didHit)
        {
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.gameObject.TryGetComponent(out Bullet bullet))
            {
                if (!bullet.GetParriable()) return;
                Vector3 parryDirection = ray.direction;
                bullet.Parry(parryDirection);
                Effects.Instance.FreezeFrame();
                Debug.Log("parrying bullet");
            }
        }
    }
}