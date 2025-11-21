using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected WeaponInstance instance;
    [SerializeField] protected Animator weaponAnimator;

    public void SetInstance(WeaponInstance inst)
    {
        instance = inst;
    }

    public abstract void HandleInput();
    public abstract void UpdateWeapon();
}