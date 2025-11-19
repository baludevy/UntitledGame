using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    public List<WeaponData> weapons = new List<WeaponData>();

    private int currentWeaponIndex = -1;

    private void Start()
    {
        if (weapons.Count > 0)
        {
            currentWeaponIndex = 0;
            UpdateWeapon(currentWeaponIndex);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CycleWeapon();
        }
    }

    public void CycleWeapon()
    {
        if (weapons.Count == 0) return;

        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;
        UpdateWeapon(currentWeaponIndex);
    }

    private void UpdateWeapon(int index)
    {
        if (index >= 0 && index < weapons.Count)
        {
            HeldWeaponController.Instance.UpdateHeldItem(weapons[index]);
        }
    }
}