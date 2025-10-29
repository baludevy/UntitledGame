using System.Collections;
using UnityEngine;
using EZCameraShake;


[CreateAssetMenu(menuName = "Ability/Dash")]
public class Dash : Ability
{
    public float force = 30f;
    public float duration = 0.15f;


    public override bool Activate()
    {
        PlayerMovement player = PlayerMovement.Instance;
        
        Rigidbody rb = player.GetRigidbody();

        // cancel the player's velocity
        rb.velocity = Vector3.zero;
        
        player.StartCoroutine(DashRoutine(rb));
        
        // shake camera
        CameraShaker.Instance?.ShakeOnce(2f, 2f, 0.05f, 0.3f);
        return true;
    }


    private IEnumerator DashRoutine(Rigidbody body)
    {
        float t = 0f;
        Vector3 dir = PlayerCamera.Instance ? PlayerCamera.Instance.transform.forward : body.transform.forward;
        while (t < duration)
        {
            body.AddForce(dir * (force / duration) * Time.deltaTime, ForceMode.VelocityChange);
            t += Time.deltaTime;
            yield return null;
        }
    }
}