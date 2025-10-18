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
    private Image staminaBar;

    // [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthBar;

    [Header("Looking at type shit Info")] public ObjectInfo objectInfo;
    public ItemInfo itemInfo;

    [Header("Hotbar")] public List<HotbarSlot> hotbarSlots;
    [SerializeField] private Transform hotbarSlotsParent;

    [Header("Inventory")] [SerializeField] public CanvasGroup inventoryHolder;
    public Transform inventorySlotsHolder;
    public List<InventorySlot> inventorySlots;

    [Header("Container Inventory")] [SerializeField]
    private CanvasGroup containerInventoryHolder;

    [SerializeField] private Transform containerSlotsHolder;
    public List<ContainerSlot> containerSlots;
    public bool containerOpen;
    public Chest openedChest;

    [Header("Misc")] public GameObject keyTip;

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

        for (int i = 0; i < containerSlotsHolder.transform.childCount; i++)
        {
            Transform row = containerSlotsHolder.transform.GetChild(i);
            for (int j = 0; j < row.childCount; j++)
            {
                ContainerSlot slot = row.GetChild(j).GetComponent<ContainerSlot>();

                slot.row = i;
                slot.col = j;

                containerSlots.Add(slot);
            }
        }
    }

    private void Update()
    {
        UpdateStaminaBar();
        UpdateHealth();
        UpdateHudInfo();

        if (Input.GetKeyDown(KeyCode.E) && containerOpen)
        {
            CloseContainerInventory();
            StartCoroutine(BlockInteractUntilKeyUp());
        }
    }
    
    IEnumerator BlockInteractUntilKeyUp()
    {
        PlayerInteract.Instance.blocked = true;
        while (Input.GetKey(KeyCode.E))
            yield return null;
        PlayerInteract.Instance.blocked = false;
    }


    private void UpdateHudInfo()
    {
        LayerMask mask = LayerMask.GetMask("DroppedItem", "Mineable");

        if (Physics.Raycast(PlayerCamera.GetRay(), out RaycastHit hit, 5f, mask))
        {
            if (hit.collider.CompareTag("Mineable"))
            {
                IMineable obj = hit.collider.GetComponent<IMineable>();

                objectInfo.SetState(true);
                objectInfo.SetObject(obj);
            }
            else if (hit.collider.CompareTag("Item"))
            {
                DroppedItem item = hit.collider.GetComponent<DroppedItem>();

                itemInfo.SetState(true);
                itemInfo.SetItem(item.itemInstance);
            }
            else
            {
                objectInfo.SetState(false);
                itemInfo.SetState(false);
            }
        }
        else
        {
            objectInfo.SetState(false);
            itemInfo.SetState(false);
        }
    }

    private void UpdateStaminaBar()
    {
        staminaBar.fillAmount = PlayerStatistics.Instance.stamina / 100f;
    }

    public void UpdateHealth()
    {
        // healthText.text = PlayerStatistics.Instance.health.ToString();
        healthBar.fillAmount = PlayerStatistics.Instance.health / 100f;
    }

    public void OpenContainerInventory(ItemInstance[,] items, Chest chest)
    {
        openedChest = chest;
        containerOpen = true;
        SetContainerInventoryState(true);
        SetInventoryState(true);
        inventoryHolder.transform.localPosition = new Vector3(0f, -185f, 0f);

        CursorManager.SetCursorLock(false);
        PlayerMovement.Instance.canLook = false;


        for (int i = 0; i < containerSlots.Count; i++)
            containerSlots[i].Clear();

        int rows = items.GetLength(0);
        int cols = items.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                ItemInstance item = items[row, col];
                if (item != null)
                {
                    int index = row * cols + col;
                    if (index < containerSlots.Count)
                        containerSlots[index].SetItem(item);
                }
            }
        }
    }

    public void CloseContainerInventory()
    {
        openedChest = null;
        containerOpen = false;
        SetContainerInventoryState(false);
        SetInventoryState(false);
        inventoryHolder.transform.localPosition = Vector3.zero;

        CursorManager.SetCursorLock(true);
        PlayerMovement.Instance.canLook = true;

        for (int i = 0; i < containerSlots.Count; i++)
            containerSlots[i].Clear();
    }

    public void UpdateContainerSlotUI(int row, int col)
    {
        if (openedChest == null) return;
        int index = row * openedChest.columns + col;
        if (index < containerSlots.Count)
            containerSlots[index].SetItem(openedChest.GetItem(row, col));
    }

    public void SetInventoryState(bool state)
    {
        inventoryHolder.alpha = state ? 1f : 0f;
        inventoryHolder.interactable = state;
        inventoryHolder.blocksRaycasts = state;
    }
    
    public void SetContainerInventoryState(bool state)
    {
        containerInventoryHolder.alpha = state ? 1f : 0f;
        containerInventoryHolder.interactable = state;
        containerInventoryHolder.blocksRaycasts = state;
    }
}