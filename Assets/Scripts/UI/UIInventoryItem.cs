using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIInventoryItem : MonoBehaviour
{
    private ItemInstance item;

    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDescription;
    [SerializeField] private TMP_Text itemStack;
    [SerializeField] private Image itemIcon;

    public void Set(ItemInstance assignedItem)
    {
        item = assignedItem;
        itemName.text = assignedItem.data.Name;
        itemDescription.text = assignedItem.data.Description;
        itemStack.text = $"x{assignedItem.amount}";
        itemIcon.sprite = assignedItem.data.Icon;
    }

    public void SetAmount(int amount)
    {
        if (item != null) item.amount = amount;
        itemStack.text = $"x{amount}";
    }

    public void Discard(int amount)
    {
        if (item == null || amount <= 0) return;

        PlayerInventory.Instance.SubtractFromItem(item, amount);
        StartCoroutine(PunchAnimation());
    }

    private IEnumerator PunchAnimation()
    {
        Vector3 original = transform.localScale;
        transform.localScale = original * 0.95f;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = original;
    }
}