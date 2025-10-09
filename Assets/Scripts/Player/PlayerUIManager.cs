using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private Image staminaBar;
    
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthBar;

    public List<PlayerInventorySlot> inventorySlots;
    [SerializeField] private Transform inventorySlotsParent;
    
    public static PlayerUIManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        for (int i = 0; i < inventorySlotsParent.childCount; i++)
        {
            inventorySlots.Add(inventorySlotsParent.GetChild(i).GetComponent<PlayerInventorySlot>());
        }
    }

    private void Update()
    {
        UpdateStaminaBar();
        UpdateHealth();
    }

    private void UpdateStaminaBar()
    {
        staminaBar.fillAmount = PlayerStatistics.Instance.stamina / 100f;
    }

    public void UpdateHealth()
    {
        healthText.text = PlayerStatistics.Instance.health.ToString();
        healthBar.fillAmount = PlayerStatistics.Instance.health / 100f;
    }
}