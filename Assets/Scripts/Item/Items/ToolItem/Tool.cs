using UnityEngine;

public class Tool : MonoBehaviour
{
    public ToolItem data;
    public Animator toolAnimator;

    public void Use()
    {
        Vector3 o = PlayerMovement.Instance.orientation.transform.position;
        
        if (Physics.Raycast(PlayerCamera.GetRay(), out RaycastHit hit, 3f))
        {
            if (hit.collider.CompareTag("Mineable"))
            {
                IMineable mineable = hit.collider.GetComponent<IMineable>();

                int damage = data.type == mineable.CanBeMinedWith ? data.damage : 0;

                Vector3 randomOffset = new Vector3(
                    Random.Range(-0.3f, 0.3f),
                    Random.Range(-0.1f, 0.1f),
                    Random.Range(-0.3f, 0.3f)
                );

                Vector3 markerPos = hit.point + randomOffset;
                DamageMarker marker = Instantiate(PlayerPrefabManager.Instance.damageMarker, markerPos,
                    Quaternion.identity).GetComponent<DamageMarker>();
                marker.Show(damage);
                
                mineable.TakeDamage(damage);
            }
        }
    }
}