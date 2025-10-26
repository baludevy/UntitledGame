using UnityEngine;

public class BowTool : Tool
{
    /* [SerializeField] private float minShootForce = 10f;
    [SerializeField] private float maxShootForce = 50f; */
    [SerializeField] private float maxChargeTime = 1f;
    [SerializeField] private Vector3 pulledLocalPos = new Vector3(0f, 0.6f, -0.8f);
    [SerializeField] private float returnSpeed = 8f;

    private float charge;
    private Vector3 startLocalPos;
    private bool charging;

    private ToolController toolController;

    private void Start()
    {
        toolController = ToolController.Instance;
        startLocalPos = transform.localPosition;
    }

    public override void HandleInput()
    {
        if (toolController.useTimer > 0)
            return;

        if (Input.GetButton("Fire1"))
        {
            if (charge < 1f)
                charge += Time.deltaTime / maxChargeTime;
        }

        if (Input.GetButtonUp("Fire1"))
        {
            ShootArrow();
            charge = 0;
        }
    }

    public override void UpdateTool()
    {
        Vector3 targetPos = Vector3.Lerp(startLocalPos, pulledLocalPos, charge);

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * returnSpeed);
    }

    private void ShootArrow()
    {
        toolController.useTimer += data.cooldown;

        /* // ItemInstance arrow = PlayerInventory.Instance.GetArrow();
        if (arrow == null) return;

        Arrow arrowData = (Arrow)arrow.data;

        Ray ray = PlayerCamera.GetRay();
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, 1000f) ? hit.point : ray.GetPoint(1000f);

        Vector3 shootPos = PlayerCamera.GetRay().origin + Vector3.down * 0.5f;
        Vector3 shootDirection = (targetPoint - shootPos).normalized;

        GameObject arrowObj =
            Instantiate(arrowData.projectilePrefab, shootPos, Quaternion.LookRotation(shootDirection));
        Rigidbody rb = arrowObj.GetComponent<Rigidbody>();


        PlayerInventory.Instance.SubtractAmountFromItem(arrow, 1);

        float force = Mathf.Lerp(minShootForce, maxShootForce, charge);
        rb.AddForce(shootDirection * force, ForceMode.Impulse); */
    }
}