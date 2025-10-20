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
    
    public bool isActive;

    private IMineable currentMineable;

    private void Start()
    {
        SetState(false);
    }

    public void SetState(bool active)
    {
        if(PlayerUIManager.Instance.itemInfo.isActive) return; 
        
        isActive = active;
        
        GetComponent<Image>().enabled = active;
        
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(active);
        }
    }
    
    public void SetObject(BaseMineable mineable)
    {
        ItemData item = mineable.DropPrefab.itemData;
        
        itemName.text = item.Name;
        itemDescription.text = item.Description;
        itemIcon.sprite = item.Icon;
        
        healthBar.fillAmount = mineable.CurrentHealth / mineable.MaxHealth;
    }
}