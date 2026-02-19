using UnityEngine;

public class AbilityController : MonoBehaviour
{
    public Ability primaryAbility;
    public Ability secondaryAbility;

    [SerializeField] private AbilitySlot primarySlot;
    [SerializeField] private AbilitySlot secondarySlot;

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

        UpdateUI();

        if (Input.GetKeyDown(KeyCode.E)) ActivateAbility(primaryAbility, ref primaryTimer);
        if (Input.GetKeyDown(KeyCode.C)) ActivateAbility(secondaryAbility, ref secondaryTimer);
    }

    private void UpdateUI()
    {
        if (primarySlot != null)
        {
            float cd = primaryAbility != null ? primaryAbility.cooldown : 0f;
            primarySlot.SetCooldown(primaryTimer, cd);
        }

        if (secondarySlot != null)
        {
            float cd = secondaryAbility != null ? secondaryAbility.cooldown : 0f;
            secondarySlot.SetCooldown(secondaryTimer, cd);
        }
    }

    private void ActivateAbility(Ability ability, ref float timer)
    {
        if (ability == null || timer > 0f) return;

        CancelOpposing(ability);

        if (ability.Activate())
        {
            timer = ability.cooldown;
            UpdateUI();
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
