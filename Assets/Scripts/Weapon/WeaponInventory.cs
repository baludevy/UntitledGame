using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    public List<WeaponData> startingWeapons = new List<WeaponData>();
    private List<WeaponInstance> weapons = new List<WeaponInstance>();
    private int currentWeaponIndex = -1;

    private void Start()
    {
        foreach (WeaponData weaponData in startingWeapons)
        {
            WeaponInstance weapon = weaponData is GunData gunData
                ? new GunInstance(gunData)
                : new WeaponInstance(weaponData);
            

            
            weapons.Add(weapon);
        }

        if (weapons.Count > 0)
        {
            currentWeaponIndex = 0;
            UpdateWeapon(currentWeaponIndex);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            CycleWeapon();
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
            HeldWeaponController.Instance.UpdateHeldItem(weapons[index]);
    }
}