using UnityEngine;

public class GunController : MonoBehaviour
{
    public Gun currentGun;
    public float useTimer;

    [SerializeField] private float recoilReturnSpeed = 10f;

    private Vector3 recoilRotation;
    private Vector3 recoilVelocity;

    public static GunController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (useTimer > 0)
            useTimer -= Time.deltaTime;

        if (!PlayerInventory.Instance.inventoryOpen && currentGun != null)
            currentGun.HandleInput();

        if (currentGun != null)
            currentGun.UpdateGun();

        UpdateRecoil();
    }

    public void AddRecoil(float amount)
    {
        float horizontal = Random.Range(-0.2f, 0.2f);
        recoilRotation += new Vector3(-amount, horizontal, 0f);
    }


    private void UpdateRecoil()
    {
        recoilRotation = Vector3.SmoothDamp(recoilRotation, Vector3.zero, ref recoilVelocity, 1f / recoilReturnSpeed);
        transform.localRotation = Quaternion.Euler(recoilRotation);
    }

    public void SetGun(Gun gun)
    {
        currentGun = gun;
    }
}