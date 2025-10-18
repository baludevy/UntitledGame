using System;
using UnityEngine;

public class Tool : MonoBehaviour
{
    public ToolInstance instance;
    public Animator toolAnimator;

    private void Start()
    {
        ToolItem data = (ToolItem)instance.data;
        float durabilityNormalized = instance.currentDurability / data.maxDurability;
        PlayerInventory.Instance.GetActiveHotbarSlot().SetFrameFill(durabilityNormalized);
    }

    public void Use()
    {
        if (Physics.Raycast(PlayerCamera.GetRay(), out RaycastHit hit, 5f))
        {
            if (hit.collider.gameObject.GetComponent<BaseMineable>())
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