using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Campfire : MonoBehaviour, IInteractable
{
    public TMP_Text infoText;
    public Image bar;

    private PlayerInventory inventory;

    private float timer = 60f;
    private float timerCap => CampfireController.timerCap;

    private bool wasLit;
    private bool lit;
    private bool wasNight;
    
    
    private void Start()
    {
        inventory = PlayerInventory.Instance;
        CampfireController.Instance.campfire = this;
        
        infoText.text = timerCap.ToString();
    }

    private void Update()
    {
        if (lit)
        {
            if (timer > 0)
                timer -= Time.deltaTime;
            else
                Extinguish();

            infoText.text = Mathf.FloorToInt(timer).ToString();
            bar.fillAmount = timer / timerCap;
        }

        bool isNight = DayNightCycle.Instance.IsNight();

        // night just started
        if (isNight && !wasNight)
        {
            if (!wasLit)
            {
                Light();
                wasLit = true;
            }
        }

        // day just started
        if (!isNight && wasNight)
        {
            Extinguish();
            wasLit = false;
        }

        wasNight = isNight;
    }

    public void Interact()
    {
        if (!lit) return;

        if (inventory.ActiveItem?.data is ResourceItem item)
        {
            if (item.resourceType == ResourceTypes.Wood && timer + item.value < timerCap)
            {
                inventory.SubtractAmountFromItem(inventory.ActiveItem, 1);
                timer += item.value;
                timer = Mathf.Min(timer, timerCap);
            }
        }
    }

    private void Light()
    {
        timer = timerCap;
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
    }

    private void OnDestroy()
    {
        CampfireController.Instance.campfire = null;
    }
}