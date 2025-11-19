using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Gun")]
public class GunData : WeaponData
{
    public float recoilAmount;
    public float knockbackAmount;
    public AudioClip shootAudio;
}