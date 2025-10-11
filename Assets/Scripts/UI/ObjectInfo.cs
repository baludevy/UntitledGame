using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectInfo : MonoBehaviour
{
    public TMP_Text itemName;
    public TMP_Text itemDescription;
    public Image itemIcon;
    public Image healthBar;

    private MineableObject currentObject;

    private void Start()
    {
        SetState(false);
    }

    public void SetState(bool active)
    {
        GetComponent<Image>().enabled = active;
        
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(active);
        }
    }
    
    public void SetObject(MineableObject newObject)
    {
        ItemData item = newObject.dropPrefab.GetComponent<DroppedItem>().itemData;
        
        itemName.text = item.Name;
        itemDescription.text = item.Description;
        itemIcon.sprite = item.Icon;
        
        healthBar.fillAmount = (float)newObject.currentHealth / (float)newObject.maxHealth;
    }
}