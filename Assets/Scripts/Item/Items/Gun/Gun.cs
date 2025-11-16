using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public GunInstance instance;
    public GunData data;

    private void Start()
    {
        data = (GunData)instance.data;
    }

    public abstract void HandleInput();
    public abstract void UpdateGun();
    public void Break() {}
}