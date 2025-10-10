using TMPro;
using UnityEngine;

public class ItemTooltip : MonoBehaviour
{
    public GameObject tooltipObject;
    
    public TMP_Text tooltipItemName;
    public TMP_Text tooltipItemDescription;

    public void Show(ItemData item, Vector3 position)
    {
        tooltipObject.transform.SetAsLastSibling();
        tooltipObject.SetActive(true);
        
        tooltipItemName.text = item.Name;
        tooltipItemDescription.text = item.Description;
    }

    public void Hide()
    {
        tooltipObject.SetActive(false);
    }
}