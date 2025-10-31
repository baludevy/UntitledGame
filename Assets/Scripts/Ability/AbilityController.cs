using UnityEngine;

public class AbilityController : MonoBehaviour
{
    public Ability primaryAbility;
    public Ability secondaryAbility;

    private float primaryTimer;
    private float secondaryTimer;

    public static AbilityController Instance;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        primaryTimer = Mathf.Max(0f, primaryTimer - Time.deltaTime);
        secondaryTimer = Mathf.Max(0f, secondaryTimer - Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Mouse3)) ActivateAbility(primaryAbility, ref primaryTimer);
        if (Input.GetKeyDown(KeyCode.C)) ActivateAbility(secondaryAbility, ref secondaryTimer);
    }

    private void ActivateAbility(Ability ability, ref float timer)
    {
        if (ability == null || timer > 0f) return;

        CancelOpposing(ability);

        if (ability.Activate())
        {
            timer = ability.cooldown;
        }
    }

    private void CancelOpposing(Ability ability)
    {
        foreach (var opp in ability.opposingAbilities)
        {
            if (opp != null)
            {
                opp.Cancel();
            }
        }
    }

    public void OnPlayerLanded()
    {
        (primaryAbility as GroundSlam)?.OnLand();
        (secondaryAbility as GroundSlam)?.OnLand();
    }

    public void OnPlayerContact()
    {
        (primaryAbility as Dash)?.OnContact();
        (secondaryAbility as Dash)?.OnContact();
    }
}