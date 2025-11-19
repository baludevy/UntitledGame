using System;
using UnityEngine;
using UnityEngine.Serialization;

public class HeldWeaponController : MonoBehaviour
{
    public Transform heldItemHolder;
    private GameObject currentWeaponObject;
    private WeaponData lastWeapon;

    private Animator itemRootAnimator;

    public static HeldWeaponController Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

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

    public void UpdateHeldItem(WeaponData weapon)
    {
        if (weapon == null)
        {
            lastWeapon = null;
            ClearHeldItem();
            return;
        }

        if (lastWeapon == weapon) return;

        if (currentWeaponObject != null)
            Destroy(currentWeaponObject);

        lastWeapon = weapon;

        if (weapon.prefab != null && heldItemHolder.childCount > 0)
        {
            itemRootAnimator.SetTrigger("Equip");
            currentWeaponObject = Instantiate(weapon.prefab, itemRootAnimator.transform);

            foreach (Transform child in currentWeaponObject.transform)
                child.gameObject.layer = LayerMask.NameToLayer("HeldItem");

            WeaponController.Instance.SetGun(currentWeaponObject.GetComponent<Weapon>());

            PlayerUIManager.Instance.SetCurrentWeaponIcon(weapon.icon);
        }
    }
}