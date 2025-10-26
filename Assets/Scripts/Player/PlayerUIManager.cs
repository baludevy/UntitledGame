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

    [SerializeField] private Transform toolbar;
    private List<ToolbarSlot> toolbarSlots = new List<ToolbarSlot>();

    private PlayerStatistics statistics;

    public static PlayerUIManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        InitializeToolbarSlots();
    }

    private void Start()
    {
        statistics = PlayerStatistics.Instance;
    }

    private void InitializeToolbarSlots()
    {
        foreach (ToolbarSlot slot in toolbar.gameObject.GetComponentsInChildren<ToolbarSlot>())
        {
            toolbarSlots.Add(slot);
        }
    }

    private void Update()
    {
        UpdateStamina();
        UpdateHealth();
    }

    #region HUD

    private void UpdateStamina()
    {
        if (staminaBar == null || staminaText == null)
        {
            Debug.LogWarning("No stamina bar or text assigned");
            return;
        }

        float stamina = statistics.Stamina.GetStamina();
        float maxStamina = statistics.Stamina.GetMaxStamina();

        staminaText.text = $"{stamina:F0}/{maxStamina:F0}";
        staminaBar.fillAmount = stamina / maxStamina;
    }

    private void UpdateHealth()
    {
        if (healthBar == null || healthText == null)
        {
            Debug.LogWarning("No health bar or text assigned");
            return;
        }

        float health = statistics.Health.GetHealth();
        float maxHealth = statistics.Health.GetMaxHealth();

        healthText.text = $"{health:F0}/{maxHealth:F0}";
        healthBar.fillAmount = health / maxHealth;
    }

    #endregion

    #region Toolbar

    public void AddToolToToolbar(Tool tool)
    {
        foreach (ToolbarSlot slot in toolbarSlots)
        {
            if (slot.GetTool() != null) continue;

            slot.SetTool(tool);

            return;
        }
    }

    public void SetActiveTool(ToolData toolData)
    {
        foreach (ToolbarSlot slot in toolbarSlots)
        {
            Tool slotTool = slot.GetTool();
            slot.SetActiveSlot(slotTool != null && slotTool.data == toolData);
        }
    }

    public void RemoveToolFromToolbar(Tool tool)
    {
        foreach (ToolbarSlot slot in toolbarSlots)
        {
            if (slot.GetTool() == tool)
                slot.SetTool(null);
        }
    }

    #endregion
}