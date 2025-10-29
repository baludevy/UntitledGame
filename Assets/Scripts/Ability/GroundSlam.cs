using UnityEngine;
using System.Collections;
using EZCameraShake;

[CreateAssetMenu(menuName = "Ability/Ground Slam")]
public class GroundSlam : Ability
{
    public float slamForce = 25f;
    public float slamSlowDuration = 0.4f;
    public float slamSlowMultiplier = 0.2f;
    public float minHeight = 10f;

    private bool slamming;

    public override bool Activate()
    {
        PlayerMovement player = PlayerMovement.Instance;
        if (player == null || player.rb == null) return false;

        if (!IsHighEnough(player)) return false;

        slamming = true;
        Rigidbody rb = player.rb;
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.down * slamForce, ForceMode.VelocityChange);
        CameraShaker.Instance.ShakeOnce(2.5f, 2.5f, 0.1f, 0.2f);

        return true;
    }

    public void OnLand()
    {
        if (!slamming) return;
        slamming = false;

        var player = PlayerMovement.Instance;
        if (player == null || player.rb == null) return;

        var rb = player.rb;
        rb.velocity = new Vector3(0f, rb.velocity.y * 0.5f, 0f);
        player.StartCoroutine(SlowdownRoutine(player));
        CameraShaker.Instance.ShakeOnce(5f, 3f, 0.1f, 0.3f);
    }

    private bool IsHighEnough(PlayerMovement player)
    {
        RaycastHit hit;
        if (Physics.Raycast(player.transform.position, Vector3.down, out hit, 100f))
            return hit.distance >= minHeight;
        return false;
    }

    private IEnumerator SlowdownRoutine(PlayerMovement player)
    {
        float originalSpeed = player.speed;
        float originalSprintSpeed = player.sprintSpeed;

        player.speed *= slamSlowMultiplier;
        player.sprintSpeed *= slamSlowMultiplier;

        yield return new WaitForSeconds(slamSlowDuration);

        player.speed = originalSpeed;
        player.sprintSpeed = originalSprintSpeed;
    }
}