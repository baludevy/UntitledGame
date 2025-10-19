using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform target;

    private void Start()
    {
        target = PlayerMovement.Instance.transform;
    }

    private void Update()
    {
        Vector3 pos = target.position;
        pos.y = transform.position.y;

        transform.LookAt(pos);
    }
}