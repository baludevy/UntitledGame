using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text staminaText;
    [SerializeField] private Image staminaBar;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthBar;

    [SerializeField] private Transform toolbar;
    private readonly List<ToolbarSlot> toolbarSlots = new();

    [SerializeField] private GameObject inventory;
    [SerializeField] private RectTransform inventoryItemHolder;
    [SerializeField] private RectTransform inventoryItemPrefab;

    private readonly Dictionary<ItemData, int> lastItemAmounts = new();
    private readonly Dictionary<ItemData, UIInventoryItem> activeItemUIs = new();

    private PlayerStatistics statistics;
    public static PlayerUIManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (ToolbarSlot slot in toolbar.GetComponentsInChildren<ToolbarSlot>())
            toolbarSlots.Add(slot);
    }

    private void Start()
    {
        statistics = PlayerStatistics.Instance;
    }

    private void Update()
    {
        UpdateStamina();
        UpdateHealth();

        var currentItems = PlayerInventory.Instance.GetItems();
        if (InventoryChanged(currentItems))
        {
            RefreshInventory(currentItems);
            Snapshot(currentItems);
        }
    }

    private bool InventoryChanged(List<ItemInstance> current)
    {
        if (current.Count != lastItemAmounts.Count) return true;
        for (int i = 0; i < current.Count; i++)
        {
            var data = current[i].data;
            var amount = current[i].amount;
            if (!lastItemAmounts.TryGetValue(data, out var last) || last != amount) return true;
        }
        return false;
    }

    private void Snapshot(List<ItemInstance> items)
    {
        lastItemAmounts.Clear();
        for (int i = 0; i < items.Count; i++)
            lastItemAmounts[items[i].data] = items[i].amount;
    }

    #region HUD

    private void UpdateStamina()
    {
        float stamina = statistics.Stamina.GetStamina();
        float maxStamina = statistics.Stamina.GetMaxStamina();
        staminaText.text = $"{stamina:F0}/{maxStamina:F0}";
        staminaBar.fillAmount = stamina / maxStamina;
    }

    private void UpdateHealth()
    {
        float health = statistics.Health.GetHealth();
        float maxHealth = statistics.Health.GetMaxHealth();
        healthText.text = $"{health:F0}/{maxHealth:F0}";
        healthBar.fillAmount = health / maxHealth;
    }

    #endregion

    #region Toolbar

    public void AddToolToToolbar(Tool tool)
    {
        for (int i = toolbarSlots.Count - 1; i >= 0; i--)
        {
            if (toolbarSlots[i].GetTool() == null)
            {
                toolbarSlots[i].SetTool(tool);
                return;
            }
        }
    }

    public void SetActiveTool(ToolData toolData)
    {
        foreach (ToolbarSlot slot in toolbarSlots)
        {
            Tool t = slot.GetTool();
            slot.SetActiveSlot(t != null && t.data == toolData);
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
    
    #region Inventory

    public void SetInventoryState(bool state)
    {
        inventory.SetActive(state);
        CursorManager.SetCursorLock(!state);
        PlayerMovement.Instance.canLook = !state;
    }

    public bool GetInventoryState() => inventory.activeSelf;

    public void RefreshInventory(List<ItemInstance> items)
    {
        var seen = new HashSet<ItemData>();

        for (int i = 0; i < items.Count; i++)
        {
            var data = items[i].data;
            var amount = items[i].amount;
            seen.Add(data);

            if (!activeItemUIs.TryGetValue(data, out var ui))
            {
                ui = Instantiate(inventoryItemPrefab.gameObject, inventoryItemHolder)
                    .GetComponent<UIInventoryItem>();
                activeItemUIs[data] = ui;
                ui.Set(items[i]);
            }
            else
            {
                ui.SetAmount(amount);
            }
        }

        var toRemove = new List<ItemData>();
        foreach (var kv in activeItemUIs)
        {
            if (!seen.Contains(kv.Key))
            {
                Destroy(kv.Value.gameObject);
                toRemove.Add(kv.Key);
            }
        }
        
        for (int i = 0; i < toRemove.Count; i++)
            activeItemUIs.Remove(toRemove[i]);

        float totalHeight = (inventoryItemPrefab.sizeDelta.y * items.Count) + 10f;
        var size = inventoryItemHolder.sizeDelta;
        size.y = totalHeight;
        inventoryItemHolder.sizeDelta = size;
    }

    #endregion
}
