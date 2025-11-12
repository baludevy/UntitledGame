using System;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;
    public Transform heldItemHolder;

    public float bobSpeed = 15f;
    public float bobMultiplier = 0.5f;

    public float swayAmount = 2f;
    public float swayPositionAmount = 1f;
    public float swaySmooth = 10f;

    public Camera cam;

    public static PlayerCamera Instance;

    private Vector3 bobOffset;
    private Vector3 desiredBob;
    private Vector3 speedBob;
    private Vector3 startPos;
    private Rigidbody rb;

    private Vector2 smoothedInput;
    private float deltaTime;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        startPos = heldItemHolder.localPosition;
        rb = PlayerMovement.Instance.GetRigidbody();
    }

    private void Update()
    {
        DamageableInfo();
    }

    private void LateUpdate()
    {
        deltaTime = Time.deltaTime;

        if (!target) return;

        UpdateBob();
        UpdateSpeedBob();

        if (PlayerMovement.Instance.GetCanLook())
            UpdateWeaponSway();

        Vector3 finalPos = startPos + bobOffset;
        heldItemHolder.localPosition = Vector3.Lerp(heldItemHolder.localPosition, finalPos, deltaTime * 15f);
        transform.position = target.position + bobOffset;
    }

    public void BobOnce(Vector3 bobDirection)
    {
        Vector3 vector = ClampVector(bobDirection * 0.15f, -3f, 3f);
        desiredBob = vector * bobMultiplier;
    }

    private void UpdateBob()
    {
        desiredBob = Vector3.Lerp(desiredBob, Vector3.zero, deltaTime * bobSpeed * 0.5f);
        bobOffset = Vector3.Lerp(bobOffset, desiredBob, deltaTime * bobSpeed);
    }

    private void UpdateSpeedBob()
    {
        if (!rb) return;
        Vector2 relativeVel = PlayerMovement.Instance.FindVelRelativeToLook();
        Vector3 v = new Vector3(relativeVel.x, rb.velocity.y, relativeVel.y) * -0.01f;
        v = Vector3.ClampMagnitude(v, 0.6f);
        speedBob = Vector3.Lerp(speedBob, v, deltaTime * 10f);
    }

    private void UpdateWeaponSway()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        Vector2 targetInput = new Vector2(mouseX, mouseY);
        smoothedInput = Vector2.Lerp(smoothedInput, targetInput, deltaTime * swaySmooth);

        float frameIndependentFactor = deltaTime * 60f;
        Quaternion targetRot = Quaternion.Euler(smoothedInput.y * swayAmount, -smoothedInput.x * swayAmount, 0);
        heldItemHolder.localRotation = Quaternion.Slerp(
            heldItemHolder.localRotation,
            targetRot,
            deltaTime * swaySmooth
        );

        Vector3 targetPos = new Vector3(
            -smoothedInput.x * swayPositionAmount,
            -smoothedInput.y * swayPositionAmount,
            0
        );

        heldItemHolder.localPosition = Vector3.Lerp(
            heldItemHolder.localPosition,
            startPos + targetPos + bobOffset,
            deltaTime * swaySmooth
        );
    }

    private Vector3 ClampVector(Vector3 vec, float min, float max)
    {
        return new Vector3(
            Mathf.Clamp(vec.x, min, max),
            Mathf.Clamp(vec.y, min, max),
            Mathf.Clamp(vec.z, min, max)
        );
    }

    IDamageable current;

    private void DamageableInfo()
    {
        if (current != null && current.Equals(null))
            current = null;

        var origin = cam.transform.position;
        var direction = cam.transform.forward;

        float radius = 0.5f;
        float distance = 20f;

        if (Physics.SphereCast(origin, radius, direction, out var hit, distance))
        {
            var damageable = hit.collider.GetComponent<IDamageable>();

            if (damageable != current)
            {
                if (current != null) current.HideCanvas();
                if (damageable != null) damageable.ShowCanvas();
                current = damageable;
            }
        }
        else
        {
            if (current != null) current.HideCanvas();
            current = null;
        }
    }

    public static Ray GetRay()
    {
        return new Ray(Instance.cam.transform.position, Instance.cam.transform.forward);
    }
}