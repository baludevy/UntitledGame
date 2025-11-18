using UnityEngine;

[CreateAssetMenu(menuName = "Items/Gun")]
public class GunData : ItemData
{
    public float damage;
    public float recoilAmount;
    public float cooldown;
    public AudioClip shootAudio;
}