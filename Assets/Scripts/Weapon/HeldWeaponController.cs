using System;
using UnityEngine;
using UnityEngine.Serialization;

public class HeldWeaponController : MonoBehaviour
{
    public Transform heldItemHolder;
    private GameObject currentWeaponObject;
    private WeaponInstance lastWeapon;
    private Animator itemRootAnimator;

    public static HeldWeaponController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        itemRootAnimator = transform.GetChild(0).GetComponent<Animator>();
    }

    private void ClearHeldItem()
    {
        if (currentWeaponObject != null)
        {
            Destroy(currentWeaponObject);
            currentWeaponObject = null;
            lastWeapon = null;
        }
    }

    public void UpdateHeldItem(WeaponInstance weapon)
    {
        if (weapon == null)
        {
            lastWeapon = null;
            ClearHeldItem();
            return;
        }

        if (lastWeapon == weapon) return;

        if (currentWeaponObject != null) Destroy(currentWeaponObject);

        lastWeapon = weapon;

        if (weapon.data.prefab != null)
        {
            itemRootAnimator.SetTrigger("Equip");
            currentWeaponObject = Instantiate(weapon.data.prefab, heldItemHolder.transform.GetChild(0));

            if (currentWeaponObject.TryGetComponent(out Weapon weaponScript))
            {
                weaponScript.SetInstance(weapon);
                WeaponController.Instance.SetGun(weaponScript);
            }
            
            PlayerUIManager.Instance.SetCurrentWeapon(weapon);
        }
    }
}