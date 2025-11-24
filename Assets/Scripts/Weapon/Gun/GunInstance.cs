using UnityEngine;

public class GunInstance : WeaponInstance
{
    public int currentAmmo;
    public int totalReserveAmmo; 
    public bool hasInfiniteMags;

    public new GunData data => base.data as GunData;

    public GunInstance(GunData gunData) : base(gunData)
    {
        currentAmmo = gunData.magSize;
        hasInfiniteMags = gunData.startingMags == 0;

        if (!hasInfiniteMags)
        {
            int spareMags = Mathf.Max(0, gunData.startingMags - 1); 
            totalReserveAmmo = spareMags * gunData.magSize;
        }
    }

    public int CurrentTotalAmmo => hasInfiniteMags ? int.MaxValue : currentAmmo + totalReserveAmmo;
}
