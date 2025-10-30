using System.Collections;
using UnityEngine;
using EZCameraShake;


[CreateAssetMenu(menuName = "Ability/Ground Slam")]
public class GroundSlam : Ability
{
    public float force = 25f;
    public float slowDuration = 0.4f;
    public float slowMultiplier = 0.2f;
    public float minHeight = 10f;
    
    private bool slamming;

    public GameObject groundSlamFX;

    public override bool Activate()
    {
        PlayerMovement player = PlayerMovement.Instance;
        Rigidbody rb = player.GetRigidbody();

        if(!IsHighEnough(player)) return false;
        
        slamming = true;
        
        // cancel the player's velocity
        rb.velocity = Vector3.zero;
        
        rb.AddForce(Vector3.down * force, ForceMode.VelocityChange);
        
        CameraShaker.Instance?.ShakeOnce(2f, 2.5f, 0.1f, 0.2f);
        return true;
    }


    public void OnLand()
    {
        if (!slamming) return;
        slamming = false;
        
        PlayerMovement player = PlayerMovement.Instance;
        Rigidbody rb = player.GetRigidbody();
        
        rb.velocity = new Vector3(0f, rb.velocity.y * 0.5f, 0f);
        
        //slow down the player for some time
        player.ChangeSprintSpeed(player.GetSprintSpeed() * slowMultiplier, slowDuration);
        player.ChangeWalkSpeed(player.GetWalkSpeed() * slowMultiplier, slowDuration);
        
        Instantiate(groundSlamFX, player.transform.position, player.transform.rotation);
        
        CameraShaker.Instance?.ShakeOnce(7f, 3f, 0.1f, 0.5f);
    }


    private bool IsHighEnough(PlayerMovement player)
    {
        // shoot a raycast down to determine the height
        return !Physics.Raycast(player.transform.position, Vector3.down, out var hit, 100f) || hit.distance >= minHeight;
    }
}