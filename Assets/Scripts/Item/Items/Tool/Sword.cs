using UnityEngine;

public class Sword : BaseTool 
{
    public override void Use()
    {
        ToolItem data = (ToolItem)instance.data;
        
        Vector3 origin = PlayerCamera.Instance.transform.position;
        Vector3 direction = PlayerCamera.Instance.transform.forward;
        float slashRange = 2f;
        float slashRadius = 3f;

        RaycastHit[] hits = Physics.SphereCastAll(origin, slashRadius, direction, slashRange);
        foreach (var slashHit in hits)
        {
            if (slashHit.collider.TryGetComponent(out BaseEnemy enemy))
                enemy.TakeDamage(data.damage);
        }

    }
}