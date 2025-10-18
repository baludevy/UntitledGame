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
    private float timerCap = 60f;

    private void Start()
    {
        inventory = PlayerInventory.Instance;
    }

    private void Update()
    {
        if(timer > 0)
            timer -= Time.deltaTime;

        infoText.text = Mathf.FloorToInt(timer).ToString();
        bar.fillAmount = timer / timerCap;
    }

    public void Interact()
    {
        if (inventory.ActiveItem?.data is ResourceItem item)
        {
            if (item.resourceType == ResourceTypes.Wood)
            {
                inventory.SubtractAmountFromItem(inventory.ActiveItem, 1);
                
                timer += item.value;
                timer = Mathf.Min(timer, timerCap);
            }
        }       
    }
}