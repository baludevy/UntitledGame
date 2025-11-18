using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform target;

    private void Start()
    {
        target = PlayerCamera.Instance.transform;
    }

    private void Update()
    {
        Vector3 pos = target.position;

        transform.LookAt(pos);
    }
}