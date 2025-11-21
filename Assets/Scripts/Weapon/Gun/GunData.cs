using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/GunData")]
public class GunData : WeaponData
{
    public int magSize;
    public int startingMags;
    public float knockbackAmount;
    public float recoilAmount;
    public AudioClip shootAudio;
}