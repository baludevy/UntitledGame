using UnityEngine;
using UnityEngine.Serialization;

public abstract class Weapon : MonoBehaviour
{
    public Animator weaponAnimator;

    public abstract void HandleInput();
    public abstract void UpdateWeapon();
}