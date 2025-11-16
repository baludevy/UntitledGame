using UnityEngine;

[CreateAssetMenu(menuName = "Items/Gun")]
public class GunData : ItemData
{
    public float recoilAmount;
    public float cooldown;
    public AudioClip shootAudio;
}