using TMPro;
using UnityEngine;

public class ItemTooltip : MonoBehaviour
{
    public GameObject tooltipObject;
    
    public TMP_Text tooltipItemName;
    public TMP_Text tooltipItemDescription;

    public void Show(ItemInstance item)
    {
        tooltipObject.transform.SetAsLastSibling();
        tooltipObject.SetActive(true);
        
        tooltipItemName.text = item.data.Name;
        tooltipItemDescription.text = item.data.Description;
    }

    public void Hide()
    {
        tooltipObject.SetActive(false);
    }
}