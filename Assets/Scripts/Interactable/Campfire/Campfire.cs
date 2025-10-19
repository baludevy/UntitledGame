using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Campfire : MonoBehaviour, IInteractable
{
    public TMP_Text infoText;
    public Image bar;

    private PlayerInventory inventory;

    private float timer;
    private float timerCap => CampfireController.timerCap;

    private bool wasLit;
    public bool lit;
    private bool wasNight;

    private void Start()
    {
        inventory = PlayerInventory.Instance;
        CampfireController.Instance.campfire = this;
        infoText.text = timerCap.ToString();
        timer = timerCap;
    }

    private void Update()
    {
        if (lit)
        {
            if (timer > 0) timer -= Time.deltaTime;
            else Extinguish();

            infoText.text = Mathf.FloorToInt(timer).ToString();
            bar.fillAmount = timer / timerCap;
        }

        bool isNight = DayNightCycle.Instance.IsNight();

        if (isNight && !wasNight)
        {
            if (!wasLit)
            {
                Light();
                wasLit = true;
            }
        }

        if (!isNight && wasNight)
        {
            Extinguish();
            wasLit = false;
        }

        wasNight = isNight;
    }

    public void Interact()
    {
        if (inventory.ActiveItem?.data is ResourceItem item && item.resourceType == ResourceTypes.Wood)
        {
            inventory.SubtractAmountFromItem(inventory.ActiveItem, 1);

            if (!lit)
                Light();

            timer += item.value;
            timer = Mathf.Min(timer, timerCap);
        }
    }

    private void Light()
    {
        if (timer <= 0) timer = timerCap;
        lit = true;
        infoText.gameObject.SetActive(true);
        bar.gameObject.SetActive(true);
        bar.transform.GetChild(0).gameObject.SetActive(true);
        GetComponent<BaseMineable>().canBeMined = false;
    }

    private void Extinguish()
    {
        lit = false;
        infoText.gameObject.SetActive(false);
        bar.gameObject.SetActive(false);
        bar.transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<BaseMineable>().canBeMined = true;
        
        PlayerStatistics.Instance.TakeDamage(100);
    }

    private void OnDestroy()
    {
        if (CampfireController.Instance != null)
            CampfireController.Instance.campfire = null;
    }
}
