using UnityEngine;

public class GunInstance : WeaponInstance
{
    public int currentAmmo;
    public int totalReserveAmmo; 

    public new GunData data => base.data as GunData;

    public GunInstance(GunData gunData) : base(gunData)
    {
        currentAmmo = gunData.magSize;
        
        int spareMags = Mathf.Max(0, gunData.startingMags - 1); 
        totalReserveAmmo = spareMags * gunData.magSize;
    }

    public int CurrentTotalAmmo => currentAmmo + totalReserveAmmo;
}