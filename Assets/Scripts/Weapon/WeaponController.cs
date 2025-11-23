using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Weapon currentWeapon;
    public float useTimer;

    public static WeaponController Instance;

    [SerializeField] private float recoilReturnSpeed = 10f;

    private Quaternion neutralRotation;
    private Quaternion spinRotation;

    private Vector3 recoilRotation;
    private Vector3 recoilVelocity;

    private float spinDuration;
    private float spinTimer;
    private bool spinActive;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        neutralRotation = transform.localRotation;
        spinRotation = Quaternion.identity;
    }


    private void Update()
    {
        if (useTimer > 0)
            useTimer -= Time.deltaTime;

        if (!PlayerInventory.Instance.inventoryOpen && currentWeapon != null)
            currentWeapon.HandleInput();

        if (currentWeapon != null)
            currentWeapon.UpdateWeapon();

        UpdateSpin();
        UpdateRecoil();
        ApplyRotation();
    }

    public void AddRecoil(float amount)
    {
        float horizontal = Random.Range(-0.2f, 0.2f);
        recoilRotation += new Vector3(-amount, horizontal, 0f);
    }

    public void SpinWeapon(float duration)
    {
        spinTimer = 0f;
        spinDuration = duration + 0.1f;
        spinActive = true;
    }

    public void ResetWeaponRotation()
    {
        spinActive = false;
        spinTimer = 0f;
        spinRotation = Quaternion.identity;
    }

    private void UpdateSpin()
    {
        if (!spinActive) return;

        spinTimer += Time.deltaTime;
        float n = spinTimer / spinDuration;

        if (n >= 1f)
        {
            spinRotation = Quaternion.identity;
            spinActive = false;
            return;
        }

        float eased = 1f - Mathf.Pow(1f - n, 3f);
        float angle = eased * 360f;
        float slowdown = 1f - Mathf.Pow(1f - eased, 10f);
        angle *= slowdown;

        spinRotation = Quaternion.AngleAxis(angle, Vector3.right);
    }

    private void UpdateRecoil()
    {
        recoilRotation = Vector3.SmoothDamp(
            recoilRotation,
            Vector3.zero,
            ref recoilVelocity,
            1f / recoilReturnSpeed
        );
    }

    private void ApplyRotation()
    {
        transform.localRotation =
            neutralRotation *
            spinRotation *
            Quaternion.Euler(recoilRotation);
    }

    public void SetGun(Weapon weapon)
    {
        currentWeapon = weapon;
    }
}