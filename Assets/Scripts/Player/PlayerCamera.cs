using System;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;
    public Transform heldItemHolder;

    [Header("Bob Settings")] public float bobSpeed = 15f;
    public float bobMultiplier = 0.5f;
    public Vector3 minimalBobOffset = new Vector3(0, 0.05f, 0);

    [Header("Weapon Sway")] public float swayAmount = 0.05f;
    public float swaySmooth = 4f;

    public Camera cam;

    public static PlayerCamera Instance;

    public Vector3 bobOffset;
    private Vector3 desiredBob;

    private Vector3 speedBob;

    private Vector3 recoilOffset;
    private Vector3 recoilRotation;
    private Vector3 recoilOffsetVel;
    private Vector3 recoilRotVel;

    private Vector3 startPos;

    private Rigidbody rb;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        heldItemHolder.position = new Vector3(heldItemHolder.position.x,
            heldItemHolder.position.y + PlayerMovement.Instance.playerHeight, heldItemHolder.position.z);
        startPos = heldItemHolder.localPosition;
        rb = PlayerMovement.Instance.rb;
    }

    private void Update()
    {
        UpdateHudInfo();
    }

    private void LateUpdate()
    {
        if (!target) return;

        UpdateBob();
        UpdateSpeedBob();
        UpdateWeaponSway();

        Vector3 finalPos = startPos + bobOffset;
        heldItemHolder.localPosition = Vector3.Lerp(heldItemHolder.localPosition, finalPos, Time.deltaTime * 15f);
        transform.position = target.position + bobOffset + minimalBobOffset;
    }

    private void UpdateHudInfo()
    {
        LayerMask mask = LayerMask.GetMask("DroppedItem", "Mineable");
        
        if (Physics.Raycast(GetRay(), out RaycastHit hit, 5f, mask))
        {
            if (hit.collider.CompareTag("Mineable"))
            {
                MineableObject obj = hit.collider.GetComponent<MineableObject>();
                ObjectInfo objInfo = PlayerUIManager.Instance.objectInfo;

                objInfo.SetState(true);
                objInfo.SetObject(obj);
            }
            else if (hit.collider.CompareTag("Item"))
            {
                Debug.Log(hit.collider.tag);
                
                DroppedItem item = hit.collider.GetComponent<DroppedItem>();
                ItemInfo itemInfo = PlayerUIManager.Instance.itemInfo;

                itemInfo.SetState(true);
                itemInfo.SetItem(item.itemInstance);
            }
            else
            {
                ObjectInfo objInfo = PlayerUIManager.Instance.objectInfo;
                objInfo.SetState(false);

                ItemInfo itemInfo = PlayerUIManager.Instance.itemInfo;
                itemInfo.SetState(false);
            }
        }
        else
        {
            ObjectInfo objInfo = PlayerUIManager.Instance.objectInfo;
            objInfo.SetState(false);

            ItemInfo itemInfo = PlayerUIManager.Instance.itemInfo;
            itemInfo.SetState(false);
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

    private void UpdateSpeedBob()
    {
        if (!rb) return;
        Vector2 relativeVel = PlayerMovement.Instance.FindVelRelativeToLook();
        Vector3 v = new Vector3(relativeVel.x, rb.velocity.y, relativeVel.y) * -0.01f;
        v = Vector3.ClampMagnitude(v, 0.6f);
        speedBob = Vector3.Lerp(speedBob, v, Time.deltaTime * 10f);
    }

    private void UpdateWeaponSway()
    {
        float mouseX = Input.GetAxis("Mouse X") * swayAmount;
        float mouseY = Input.GetAxis("Mouse Y") * swayAmount;
        Quaternion targetRot = Quaternion.Euler(mouseY, -mouseX, 0);
        heldItemHolder.localRotation =
            Quaternion.Slerp(heldItemHolder.localRotation, targetRot, Time.deltaTime * swaySmooth);
    }

    private Vector3 ClampVector(Vector3 vec, float min, float max)
    {
        return new Vector3(
            Mathf.Clamp(vec.x, min, max),
            Mathf.Clamp(vec.y, min, max),
            Mathf.Clamp(vec.z, min, max)
        );
    }

    public static Ray GetRay()
    {
        return new Ray(Instance.cam.transform.position, Instance.cam.transform.forward);
    }
}