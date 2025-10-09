using System;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;

    [Header("Bob Settings")]
    public float bobSpeed = 15f;
    public float bobMultiplier = 0.5f;

    public Vector3 bobOffset;
    private Vector3 desiredBob;
    
    public static PlayerCamera Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else 
            Destroy(gameObject);
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            UpdateBob();
            transform.position = target.position + bobOffset;
        }
    }

    public void BobOnce(Vector3 bobDirection)
    {
        Vector3 vector = ClampVector(bobDirection * 0.15f, -3f, 3f);
        desiredBob = vector * bobMultiplier;
    }

    private void UpdateBob()
    {
        desiredBob = Vector3.Lerp(desiredBob, Vector3.zero, Time.deltaTime * bobSpeed * 0.5f);
        bobOffset = Vector3.Lerp(bobOffset, desiredBob, Time.deltaTime * bobSpeed);
    }

    private Vector3 ClampVector(Vector3 vec, float min, float max)
    {
        return new Vector3(
            Mathf.Clamp(vec.x, min, max),
            Mathf.Clamp(vec.y, min, max),
            Mathf.Clamp(vec.z, min, max)
        );
    }
}