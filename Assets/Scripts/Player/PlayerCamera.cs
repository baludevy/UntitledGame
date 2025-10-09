using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;

    private void LateUpdate()
    {
        if (target != null)
            transform.position = target.position;
    }
}