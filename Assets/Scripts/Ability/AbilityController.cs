using System;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    public Ability primaryAbility;
    public Ability secondaryAbility;

    [NonSerialized] public float primaryTimer;
    [NonSerialized] public float secondaryTimer;

    public static AbilityController Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else

            Destroy(this);
    }

    private void Update()
    {
        if (primaryTimer > 0) primaryTimer -= Time.deltaTime;
        if (secondaryTimer > 0) secondaryTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.F) && primaryAbility != null && primaryTimer <= 0)
        {
            if(primaryAbility.Activate())
                primaryTimer = primaryAbility.cooldown;
        }

        if (Input.GetKeyDown(KeyCode.C) && secondaryAbility != null && secondaryTimer <= 0)
        {
            if(secondaryAbility.Activate())
                secondaryTimer = secondaryAbility.cooldown;
        }
    }

    public void OnPlayerLanded()
    {
        if (primaryAbility is GroundSlam gs1) gs1.OnLand();
        if (secondaryAbility is GroundSlam gs2) gs2.OnLand();
    }
}