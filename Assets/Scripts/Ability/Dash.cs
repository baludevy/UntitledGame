using System.Collections;
using UnityEngine;
using EZCameraShake;

[CreateAssetMenu(menuName = "Ability/Dash")]
public class Dash : Ability
{
    public float dashForce = 30f;
    public float dashDuration = 0.15f;

    public override bool Activate()
    {
        PlayerMovement player = PlayerMovement.Instance;
        if (player == null || player.rb == null) return false;

        Rigidbody rb = player.rb;
        rb.velocity = Vector3.zero;

        player.StartCoroutine(DashRoutine(rb));
        CameraShaker.Instance.ShakeOnce(2f, 2f, 0.05f, 0.15f);

        return true;
    }

    private IEnumerator DashRoutine(Rigidbody rb)
    {
        float time = 0f;
        while (time < dashDuration)
        {
            rb.AddForce(PlayerCamera.Instance.transform.forward * (dashForce / dashDuration) * Time.deltaTime,
                ForceMode.VelocityChange);
            time += Time.deltaTime;
            yield return null;
        }
    }
}