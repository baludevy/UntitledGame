using System;
using UnityEngine;

public class Tool : MonoBehaviour
{
    public ToolInstance instance;
    public Animator toolAnimator;
    private Camera camera;

    private void Start()
    {
        camera = Camera.main;
        ToolItem data = (ToolItem)instance.data;
        float durabilityNormalized = instance.currentDurability / data.maxDurability;
        PlayerInventory.Instance.GetActiveHotbarSlot().SetFrameFill(durabilityNormalized);
    }

    public void Use()
    {
        ToolItem data = (ToolItem)instance.data;
        
        if (data.type == ToolType.Sword)
        {
            Vector3 origin = camera.transform.position;
            Vector3 direction = camera.transform.forward;
            float slashRange = 1.5f;
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
                IMineable mineable = hit.collider.GetComponent<IMineable>();
                
                if(!mineable.CanBeMined) return;
                
                int damage = data.type == mineable.CanBeMinedWith ? data.damage : 0;
                
                if(damage > 0) 
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