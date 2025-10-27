using System;
using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    private float stamina = 100f;

    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaLoss = 10f;
    [SerializeField] private float jumpStaminaLoss = 7f;
    [SerializeField] private float staminaRegen = 6f;
    [SerializeField] private float regenDelay = 1.5f;

    private float regenTimer;

    private void Start()
    {
        stamina = maxStamina;
    }

    private void Update()
    {
        if (regenTimer > 0f)
        {
            regenTimer -= Time.deltaTime;
            return;
        }

        if (stamina < maxStamina)
        {
            stamina += staminaRegen * Time.deltaTime;
            if (stamina > maxStamina) stamina = maxStamina;
        }
    }

    public void UseStamina(float amount)
    {
        stamina = Mathf.Max(stamina - amount, 0f);
        regenTimer = regenDelay;
    }

    public void UseJumpStamina()
    {
        UseStamina(jumpStaminaLoss);
    }

    public void SetMaxStamina(float newMax)
    {
        maxStamina = newMax;
        if (stamina > maxStamina) stamina = maxStamina;
    }

    public float GetStamina() => stamina;
    public float GetMaxStamina() => maxStamina;
    public float GetStaminaLoss() => staminaLoss;
    public float GetJumpStaminaLoss() => jumpStaminaLoss;
    public float GetStaminaRegen() => staminaRegen;
    public float GetRegenDelay() => regenDelay;
}