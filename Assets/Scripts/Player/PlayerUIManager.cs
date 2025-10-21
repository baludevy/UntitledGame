using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [Header("Player UI Elements")] public GameObject inventory;
    public GameObject crosshair;
    public GameObject hud;

    [Header("HUD")] [Header("Stats")] [SerializeField]
    private TMP_Text staminaText;

    [SerializeField] private Image staminaBar;

    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthBar;

    [Header("Looking at type shit Info")] public ObjectInfo objectInfo;
    public ItemInfo itemInfo;

    [Header("Hotbar")] public List<HotbarSlot> hotbarSlots;
    [SerializeField] private Transform hotbarSlotsParent;

    [Header("Inventory")] [SerializeField] public CanvasGroup inventoryHolder;
    public Transform inventorySlotsHolder;
    [NonSerialized] public List<InventorySlot> inventorySlots;

    [Header("Misc")] public GameObject keyTip;

    [Header("Damage /flash effect")] [SerializeField]
    private Image effectImage;

    private Coroutine periodicFlashCoroutine;

    public TMP_Text dayCounterText;

    public static PlayerUIManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        hotbarSlots = new List<HotbarSlot>();
        inventorySlots = new List<InventorySlot>();

        for (int i = 0; i < hotbarSlotsParent.childCount; i++)
        {
            hotbarSlots.Add(hotbarSlotsParent.GetChild(i).GetComponent<HotbarSlot>());
        }

        for (int i = 0; i < inventorySlotsHolder.childCount; i++)
        {
            Transform row = inventorySlotsHolder.GetChild(i);
            for (int j = 0; j < row.childCount; j++)
            {
                InventorySlot slot = row.GetChild(j).GetComponent<InventorySlot>();

                slot.row = i;
                slot.col = j;

                inventorySlots.Add(slot);
            }
        }
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
            Debug.LogWarning("No day counter text assigned");
            return;
        }

        dayCounterText.text = DayNightCycle.CurrentDay.ToString();
    }

    public void SetInventoryState(bool state)
    {
        inventoryHolder.alpha = state ? 1f : 0f;
        inventoryHolder.interactable = state;
        inventoryHolder.blocksRaycasts = state;
    }
}