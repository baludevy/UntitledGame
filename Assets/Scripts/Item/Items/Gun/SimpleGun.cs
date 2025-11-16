using UnityEngine;

public class SimpleGun : Gun
{
    public override void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && GunController.Instance.useTimer <= 0)
        {
            GunController.Instance.AddRecoil(data.recoilAmount);
            AudioManager.Play(data.shootAudio, Vector3.zero, 0.9f, 1.1f, 0.3f, false);

            GunController.Instance.useTimer = data.cooldown;
        }
    }

    public override void UpdateGun() {}
}