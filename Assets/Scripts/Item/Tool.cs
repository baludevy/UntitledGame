using UnityEngine;

public class Tool : MonoBehaviour
{
    public Pickaxe data;
    public Animator toolAnimator;

    public void Use()
    {
        Vector3 o = PlayerMovement.Instance.orientation.transform.position;
        
        if (Physics.Raycast(PlayerCamera.GetRay(), out RaycastHit hit, 3f))
        {
            if (hit.collider.CompareTag("Mineable"))
            {
                MineableObject obj = hit.collider.GetComponent<MineableObject>();
                
                obj.Mine(data.damage);
            }
        }
    }
}