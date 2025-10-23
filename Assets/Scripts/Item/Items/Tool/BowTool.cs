using UnityEngine;

public class BowTool : BaseTool
{
    [SerializeField] private float minShootForce = 10f;
    [SerializeField] private float maxShootForce = 50f;
    private float charge;

    public override void HandleInput()
    {
        if (Input.GetButton("Fire1"))
        {
            if (charge < 1f)
                charge += Time.deltaTime;
        }

        if (Input.GetButtonUp("Fire1"))
        {
            ShootArrow();
            charge = 0;
        }
    }

    public override void UpdateTool()
    {
    }

    private void ShootArrow()
    {
        ItemInstance arrow = PlayerInventory.Instance.GetArrow();
        if (arrow == null) return;

        Arrow arrowData = (Arrow)arrow.data;

        Ray ray = PlayerCamera.GetRay();
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(1000f);

        Vector3 shootPos = PlayerCamera.GetRay().origin + Vector3.down * 0.5f;
        
        Vector3 shootDirection = (targetPoint - shootPos).normalized;
        
        GameObject arrowObj = Instantiate(arrowData.projectilePrefab, shootPos, Quaternion.LookRotation(shootDirection));
        Rigidbody rb = arrowObj.GetComponent<Rigidbody>();

        float force = Mathf.Lerp(minShootForce, maxShootForce, charge);
        rb.AddForce(shootDirection * force, ForceMode.Impulse);
    }

}