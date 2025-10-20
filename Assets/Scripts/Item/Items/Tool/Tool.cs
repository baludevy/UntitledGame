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
        ToolItem data = (ToolItem)instance.data;
        
        if (data.type == ToolType.Sword)
        {
            Vector3 origin = PlayerCamera.Instance.transform.position;
            Vector3 direction = PlayerCamera.Instance.transform.forward;
            float slashRange = 2f;
            float slashRadius = 3f;

            RaycastHit[] hits = Physics.SphereCastAll(origin, slashRadius, direction, slashRange);
            foreach (var slashHit in hits)
            {
                if (slashHit.collider.TryGetComponent(out BaseEnemy enemy))
                {
                    enemy.TakeDamage(data.damage);
                }
            }
            
            return;
        }
        
        if (Physics.Raycast(PlayerCamera.GetRay(), out RaycastHit hit, 5f))
        {
            if (hit.collider.gameObject.GetComponent<IMineable>() != null)
            {
                BaseMineable mineable = hit.collider.GetComponent<BaseMineable>();
                
                if(!mineable.canBeMined) return;
                
                int damage = data.type == mineable.CanBeMinedWith ? data.damage : 0;
                
                /* if(damage > 0) 
                    instance.TakeDurability(); */
                
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