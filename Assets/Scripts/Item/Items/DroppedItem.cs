using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public ItemData itemData;
    private ItemInstance itemInstance;

    private Collider playerCollider;
    private float ignoreUntil;

    private void Awake()
    {
        if(itemInstance != null) return;
        
        if (itemData is ToolData tool)
            itemInstance = new ToolInstance(tool);
        else if(itemData is PlaceableData placeable)
            itemInstance = new PlaceableInstance(placeable);
        else
            itemInstance = new ItemInstance(itemData);
    }

    public void Initialize(ItemInstance instance, bool ignore, Collider dropperCollider = null)
    {
        itemInstance = instance;

        playerCollider = dropperCollider;

        if (ignore)
        {
            ignoreUntil = Time.time + 0.5f;

            if (playerCollider != null)
                Physics.IgnoreCollision(GetComponent<Collider>(), playerCollider, true);
        }
    }

    private void Update()
    {
        if (playerCollider != null && Time.time >= ignoreUntil)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), playerCollider, false);
            playerCollider = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time < ignoreUntil) return;

        PickupItem();
    }

    private void PickupItem()
    {
        PlayerInventory.Instance.AddItem(itemInstance);
        itemInstance.OnPickup();
        Destroy(gameObject);
    }
}