using UnityEngine;

public class Tool : MonoBehaviour
{
    public ToolInstance instance;
    public Animator toolAnimator;
    
    public void Use()
    {
        if (Physics.Raycast(PlayerCamera.GetRay(), out RaycastHit hit, 3f))
        {
            if (hit.collider.CompareTag("Mineable"))
            {
                IMineable mineable = hit.collider.GetComponent<IMineable>();

                ToolItem data = (ToolItem)instance.data;
                
                int damage = data.type == mineable.CanBeMinedWith ? data.damage : 0;
                
                instance.TakeDurability();
                
                mineable.TakeDamage(damage);
            }
        }
    }

    public void Break()
    {
        PlayerInventory.Instance.RemoveItemByID(instance.id);
        Destroy(gameObject);
    }
}