using UnityEngine;
using UnityEngine.Serialization;

public class WeaponController : MonoBehaviour
{
    public Weapon currentWeapon;
    public float useTimer;

    [SerializeField] private float recoilReturnSpeed = 10f;

    private Vector3 recoilRotation;
    private Vector3 recoilVelocity;

    public static WeaponController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (useTimer > 0)
            useTimer -= Time.deltaTime;

        if (!PlayerInventory.Instance.inventoryOpen && currentWeapon != null)
            currentWeapon.HandleInput();

        if (currentWeapon != null)
            currentWeapon.UpdateWeapon();

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

    public void SetGun(Weapon weapon)
    {
        currentWeapon = weapon;
    }
}