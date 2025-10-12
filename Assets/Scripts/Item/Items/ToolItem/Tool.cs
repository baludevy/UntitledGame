using UnityEngine;

public class Tool : MonoBehaviour
{
    public ToolItem data;
    public Animator toolAnimator;

    public void Use()
    {
        if (Physics.Raycast(PlayerCamera.GetRay(), out RaycastHit hit, 3f))
        {
            if (hit.collider.CompareTag("Mineable"))
            {
                IMineable mineable = hit.collider.GetComponent<IMineable>();

                int damage = data.type == mineable.CanBeMinedWith ? data.damage : 0;
                
                mineable.TakeDamage(damage);
            }
        }
    }
}