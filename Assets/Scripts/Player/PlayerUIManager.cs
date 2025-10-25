using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public GameObject inventory;
    public GameObject crosshair;

    [SerializeField] private TMP_Text staminaText;
    [SerializeField] private Image staminaBar;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthBar;

    private Coroutine periodicFlashCoroutine;

    public TMP_Text dayCounterText;

    public static PlayerUIManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        UpdateStamina();
        UpdateHealth();
        UpdateDayCounter();
    }


    private void UpdateStamina()
    {
        if (staminaBar == null || staminaText == null)
        {
            Debug.LogWarning("No stamina bar or text assigned");
            return;
        }

        staminaText.text = $"{PlayerStatistics.Instance.stamina:F0}/100";
        staminaBar.fillAmount = PlayerStatistics.Instance.stamina / 100f;
    }

    private void UpdateHealth()
    {
        if (healthBar == null || healthText == null)
        {
            Debug.LogWarning("No health bar or text assigned");
            return;
        }

        healthText.text = $"{PlayerStatistics.Instance.health:F0}/{100}";
        healthBar.fillAmount = PlayerStatistics.Instance.health / 100f;
    }

    private void UpdateDayCounter()
    {
        if (dayCounterText == null)
        {
            // Debug.LogWarning("No day counter text assigned");
            return;
        }

        dayCounterText.text = DayNightCycle.CurrentDay.ToString();
    }
}