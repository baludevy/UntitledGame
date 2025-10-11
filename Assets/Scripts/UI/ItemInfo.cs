using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    public TMP_Text itemName;
    public TMP_Text itemDescription;
    public TMP_Text stackText;
    public Image itemIcon;

    public bool isActive;
    
    private MineableObject currentObject;

    private void Start()
    {
        SetState(false);
    }

    public void SetState(bool active)
    {
        if(PlayerUIManager.Instance.objectInfo.isActive) return; 
        
        isActive = active;
        
        GetComponent<Image>().enabled = active;
        
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(active);
        }
    }
    
    public void SetItem(ItemInstance newItem)
    {
        itemName.text = newItem.data.Name;
        itemDescription.text = newItem.data.Description;
        itemIcon.sprite = newItem.data.Icon;
        stackText.text = newItem.data.Stackable ? newItem.stack.ToString() : "";
    }
}