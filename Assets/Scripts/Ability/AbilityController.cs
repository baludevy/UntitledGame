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


        if (Input.GetKeyDown(KeyCode.Mouse3)) ActivatePrimary();
        if (Input.GetKeyDown(KeyCode.C)) ActivateSecondary();
    }

    private void ActivatePrimary()
    {
        if (primaryAbility == null || primaryTimer > 0f) return;
        if (primaryAbility.Activate()) primaryTimer = primaryAbility.cooldown;
    }

    private void ActivateSecondary()
    {
        if (secondaryAbility == null || secondaryTimer > 0f) return;
        if (secondaryAbility.Activate()) secondaryTimer = secondaryAbility.cooldown;
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