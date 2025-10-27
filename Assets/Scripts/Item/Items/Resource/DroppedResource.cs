using UnityEngine;

public class DroppedResource : MonoBehaviour
{
    private ItemInstance item;
    private Vector3 velocity;
    public float acceleration = 25f;
    public float baseSpeed = 5f;
    public bool moveToPlayer;

    public void SetItem(ItemInstance newItem)
    {
        item = newItem;
        transform.LookAt(PlayerMovement.Instance.transform);
        moveToPlayer = true;
    }

    private void Update()
    {
        if (!moveToPlayer || PlayerMovement.Instance == null) return;

        Vector3 targetPos = PlayerMovement.Instance.itemPickup.position;
        Vector3 dir = (targetPos - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPos);
        float playerSpeed = PlayerMovement.Instance.rb.velocity.magnitude;
        float adjustedSpeed = baseSpeed + (playerSpeed * 0.8f) + (distance * 0.5f);
        velocity = Vector3.Lerp(velocity, dir * adjustedSpeed, acceleration * Time.deltaTime);
        transform.position += velocity * Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("ItemPickup"))
        {
            PlayerInventory.Instance.AddItem(item);
            Destroy(gameObject);
        }
    }
}