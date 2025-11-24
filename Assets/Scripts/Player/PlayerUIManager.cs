using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [Header("HUD")] [Header("Stats")] [SerializeField]
    private TMP_Text staminaText;

    [SerializeField] private Image staminaBar;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthBar;

    [Header("Inventory")] [SerializeField] public CanvasGroup inventoryHolder;
    public Transform inventorySlotsHolder;
    [NonSerialized] public List<InventorySlot> inventorySlots;

    [Header("Weapon stuff")] [SerializeField]
    private Transform weaponListHolder;

    private List<WeaponListItem> weaponListItems = new List<WeaponListItem>();
    private WeaponInstance currentWeaponInstance;
    private List<Image> magIcons = new List<Image>();
    private PlayerStatistics statistics;

    [SerializeField] private GameObject weaponListItemPrefab;

    [Header("Ammo UI")] [SerializeField] private GameObject ammoUI;
    [SerializeField] private TMP_Text totalAmmoText;
    [SerializeField] private Transform magHolder;
    [SerializeField] private Sprite bulletSprite;
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private GameObject infiniteIcon;

    public static PlayerUIManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        inventorySlots = new List<InventorySlot>();
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
        if (statistics == null) statistics = PlayerStatistics.Instance;
    }

    private void LateUpdate()
    {
        UpdateHealth();

        if (currentWeaponInstance is GunInstance gun)
        {
            UpdateAmmoUI(gun);
            ammoUI.SetActive(true);
        }
        else
            ammoUI.SetActive(false);
    }

    private void UpdateHealth()
    {
        if (healthBar == null || healthText == null) return;

        float health = statistics.Health.GetHealth();
        float maxHealth = statistics.Health.GetMaxHealth();

        healthText.text = $"{health:F0}/{maxHealth:F0}";
        healthBar.fillAmount = health / maxHealth;
    }

    public void AddWeaponToUI(WeaponInstance instance)
    {
        GameObject createdItem = Instantiate(weaponListItemPrefab, weaponListHolder);
        WeaponListItem item = createdItem.GetComponent<WeaponListItem>();

        bool first = weaponListItems.Count == 0;
        item.Initialize(first, instance.data.icon, instance);

        weaponListItems.Add(item);

        if (first) currentWeaponInstance = instance;
    }

    public void SetCurrentWeapon(WeaponInstance instance)
    {
        currentWeaponInstance = instance;

        if (instance.data is GunData)
            SetupAmmoUI((GunInstance)instance);

        for (int i = 0; i < weaponListItems.Count; i++)
        {
            bool isActive = weaponListItems[i].Matches(instance);
            weaponListItems[i].SetState(isActive);
        }
    }

    private void SetupAmmoUI(GunInstance gun)
    {
        for (int i = 0; i < magHolder.childCount; i++)
            Destroy(magHolder.GetChild(i).gameObject);

        magIcons.Clear();
        infiniteIcon.SetActive(gun.hasInfiniteMags);

        for (int i = 0; i < gun.data.magSize; i++)
        {
            GameObject icon = new GameObject("bullet", typeof(Image));
            icon.GetComponent<RectTransform>().sizeDelta = new Vector2(32, 32);
            icon.transform.SetParent(magHolder, false);

            Image img = icon.GetComponent<Image>();
            img.sprite = bulletSprite;

            magIcons.Add(img);
        }


        UpdateAmmoUI(gun);
    }

    public void UpdateAmmoUI(GunInstance gun)
    {
        totalAmmoText.text = gun.hasInfiniteMags ? "" : $"{gun.CurrentTotalAmmo}/{gun.data.TotalAmmo}";

        for (int i = 0; i < magIcons.Count; i++)
            magIcons[i].sprite = i < gun.currentAmmo ? bulletSprite : emptySprite;
    }


    public void SetInventoryState(bool state)
    {
        inventoryHolder.alpha = state ? 1f : 0f;
        inventoryHolder.interactable = state;
        inventoryHolder.blocksRaycasts = state;
    }

    public bool GetInventoryState()
    {
        return inventoryHolder.alpha > 0f;
    }
}