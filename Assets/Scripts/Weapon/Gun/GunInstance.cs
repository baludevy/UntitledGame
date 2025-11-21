public class GunInstance : WeaponInstance
{
    public int currentAmmo;
    public int currentMagCount;

    public new GunData data => base.data as GunData;

    public GunInstance(GunData gunData) : base(gunData)
    {
        currentAmmo = gunData.magSize;
        currentMagCount = gunData.startingMags - 1; // subtract 1 to account for the already loaded mag
    }
}