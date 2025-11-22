using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/GunData")]
public class GunData : WeaponData
{
    public int magSize = 10;
    public int startingMags = 1;
    public float reloadTime = 1f;
    public bool automatic;
    public float knockbackAmount;
    public float recoilAmount = 30;
    public AudioClip shootAudio;

    public int TotalAmmo => magSize * startingMags;
}