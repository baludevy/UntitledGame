using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory.Instance.AddItem(itemData);
            itemData.OnPickup();
            Destroy(gameObject);
        }
    }
}