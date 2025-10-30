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

        PerformDash(rb);

        // shake camera
        CameraShaker.Instance?.ShakeOnce(2f, 2f, 0.05f, 0.3f);
        return true;
    }

    private void PerformDash(Rigidbody rb)
    {
        bool playerGrounded = PlayerMovement.Instance.IsGrounded();
        Vector2 movingDir = PlayerMovement.Instance.GetInputDirection();

        // base direction is forward
        Vector3 targetDir = PlayerCamera.Instance.transform.forward;

        if (movingDir.x > 0)
        {
            // if the player is moving right dash right
            targetDir = PlayerCamera.Instance.transform.right;

            //if the player is in the air, send him upwards a little bit
            if (!playerGrounded)
                targetDir += Vector3.up * 0.3f;
        }
        else if (movingDir.x < 0)
        {
            // if the player is moving right dash left
            targetDir = -PlayerCamera.Instance.transform.right;

            //if the player is in the air, send him upwards a little bit
            if (!playerGrounded)
                targetDir += Vector3.up * 0.3f;
        }
        else if (movingDir.y < 0)
        {
            // dash backwards
            targetDir = -PlayerCamera.Instance.transform.forward;
        }

        // normalize so distance is consistent in all directions
        targetDir = targetDir.normalized;
        
        // flatten vertical tilt for consistent dash height
        targetDir.y = playerGrounded ? 0f : targetDir.y;
        targetDir = targetDir.normalized;

        //if the player is on the ground then multiply the force by the current drag
        float adjustedForce = playerGrounded ? force * PlayerMovement.Instance.GetDrag() : force;

        // compute the dash velocity once, ensuring consistent distance
        Vector3 dashVelocity = targetDir * adjustedForce;

        rb.velocity = dashVelocity;
    }
}