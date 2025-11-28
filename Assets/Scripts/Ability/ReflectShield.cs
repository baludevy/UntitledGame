using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/ReflectShield")]
public class ReflectShield : Ability
{
    public float duration = 1.0f;

    private Coroutine shieldCoroutine;
    private PlayerMovement playerMovement;

    public override bool Activate()
    {
        playerMovement = PlayerMovement.Instance;

        if (playerMovement == null || PlayerStatistics.Instance.Health == null) return false;

        if (shieldCoroutine != null)
        {
            playerMovement.StopCoroutine(shieldCoroutine);
        }

        shieldCoroutine = playerMovement.StartCoroutine(ShieldRoutine());

        return true;
    }

    private IEnumerator ShieldRoutine()
    {
        PlayerStatistics.Instance.Health.SetImmune(true);

        yield return new WaitForSeconds(duration);

        if (PlayerStatistics.Instance.Health != null)
        {
            PlayerStatistics.Instance.Health.SetImmune(false);
        }

        shieldCoroutine = null;
    }

    public override void Cancel()
    {
        if (shieldCoroutine != null && playerMovement != null)
        {
            playerMovement.StopCoroutine(shieldCoroutine);
        }

        if (PlayerStatistics.Instance.Health != null)
        {
            PlayerStatistics.Instance.Health.SetImmune(false);
        }

        shieldCoroutine = null;
    }
}