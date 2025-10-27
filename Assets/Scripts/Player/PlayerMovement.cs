using UnityEngine;
using System;
using EZCameraShake;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;

    [Header("Movement Properties")] [SerializeField]
    private float speed = 1000f;

    [SerializeField] private float sprintSpeed = 1750f;
    [SerializeField] private float jumpForce = 300f;
    [SerializeField] private float groundDrag = 1f;
    [SerializeField] private float airMultiplier = 0.6f;
    [SerializeField] public float playerHeight = 2f;
    [SerializeField] private LayerMask whatIsGround;

    private bool readyToJump = true;
    private bool grounded;
    private bool surfing;
    private const float jumpCooldown = 0.25f;
    private Vector3 normalVector;
    private const float maxSlopeAngle = 35f;
    private float fallSpeed;

    [Header("Input")] private float x;
    private float y;
    private bool jumping;
    private bool sprinting;

    [Header("Mouse Look")] public float sensitivity = 50f;
    public float sensMultiplier = 1f;
    private float xRotation = 0f;
    public float desiredX;
    public Transform orientation;
    public Transform camTransform;
    public bool canLook = true;

    [Header("Camera")] public float defaultFOV = 85f;
    public float sprintFOV = 95f;

    [Header("Effects")] private float walkBobTimer = 0f;
    private float bobSpeed = 8f;
    private const float bobAmount = 0.8f;

    [Header("Footsteps")] [SerializeField] private float footstepInterval = 0.4f;
    private float adjustedFootstepInterval;
    private float footstepTimer;

    [Header("Misc")] public Transform itemPickup;

    public ParticleSystem ps;
    private ParticleSystem.EmissionModule emission;

    private PlayerStamina stamina;

    public static PlayerMovement Instance;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (orientation == null) orientation = transform;
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        if (camTransform == null && Camera.main != null) camTransform = Camera.main.transform;
    }

    private void Start()
    {
        desiredX = orientation.localEulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        emission = ps.emission;
        stamina = PlayerStatistics.Instance.Stamina;
    }

    private void Update()
    {
        fallSpeed = rb.velocity.y;
        GetInput();
        HandleDrag();
        HandleLook();
        WalkBob();

        if (readyToJump && grounded && jumping && stamina.GetStamina() >= stamina.GetJumpStaminaLoss())
        {
            readyToJump = false;
            Invoke(nameof(ResetJump), jumpCooldown);
            Jump();
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void WalkBob()
    {
        if (!grounded) return;

        Vector3 horizontalVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        float moveSpeed = horizontalVel.magnitude;

        if (moveSpeed > 0.01f && PlayerCamera.Instance != null)
        {
            walkBobTimer += Time.deltaTime * bobSpeed;
            float xBob = Mathf.Sin(walkBobTimer) * bobAmount;
            float yBob = Mathf.Cos(walkBobTimer * 2f) * bobAmount;
            PlayerCamera.Instance.BobOnce(new Vector3(xBob, yBob, 0f));

            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                AudioClip clip = GetCurrentFootstepClip();
                if (clip != null) AudioManager.Play(clip, transform.position, 0.7f, 1.3f, 0.1f, false);
                footstepTimer = adjustedFootstepInterval;
            }
        }
    }

    private AudioClip GetCurrentFootstepClip()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10f, whatIsGround))
        {
            GroundSurface surface = hit.collider.GetComponent<GroundSurface>();
            if (surface != null) return surface.footstepClip;
        }

        return null;
    }

    private void HandleLook()
    {
        float mouseX = canLook ? Input.GetAxis("Mouse X") * sensitivity * sensMultiplier : 0f;
        float mouseY = canLook ? Input.GetAxis("Mouse Y") * sensitivity * sensMultiplier : 0f;

        desiredX += mouseX;
        desiredX = Mathf.Repeat(desiredX, 360f);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (camTransform != null) camTransform.localRotation = Quaternion.Euler(xRotation, desiredX, 0f);
        orientation.Rotate(Vector3.up * mouseX);
    }

    private void HandleDrag()
    {
        rb.drag = grounded ? groundDrag : 0f;
    }

    private void GetInput()
    {
        if (PlayerUIManager.Instance.GetInventoryState())
        {
            x = 0;
            y = 0;
            jumping = false;
            sprinting = false;
            return;
        }

        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        sprinting = Input.GetButton("Sprint");
    }

    private void Movement()
    {
        rb.AddForce(Vector3.down * (Time.deltaTime * 12.5f));

        Vector2 mag = FindVelRelativeToLook();
        float xVelLook = mag.x;
        float yVelLook = mag.y;

        CounterMovement(x, y, mag);

        float maxSpeed = 25f;
        float moveSpeed = speed;

        bool isMoving = x != 0 || y != 0;
        bool isSprinting = sprinting && isMoving && stamina.GetStamina() > 0f;

        if (isSprinting)
        {
            moveSpeed = sprintSpeed;
            stamina.UseStamina(stamina.GetStaminaLoss() * Time.deltaTime);
            SpeedLines();
            FovEffect();
            adjustedFootstepInterval = footstepInterval / 1.25f;
        }
        else
        {
            adjustedFootstepInterval = footstepInterval;
            ResetSprintingEffects();
        }

        if (x > 0f && xVelLook > maxSpeed) x = 0f;
        if (x < 0f && xVelLook < -maxSpeed) x = 0f;
        if (y > 0f && yVelLook > maxSpeed) y = 0f;
        if (y < 0f && yVelLook < -maxSpeed) y = 0f;

        float forwardMultiplier = grounded ? 1f : airMultiplier;
        float sideMultiplier = grounded ? 1f : airMultiplier;

        if (surfing)
        {
            forwardMultiplier = 0.7f;
            sideMultiplier = 0.3f;
        }

        rb.AddForce(orientation.forward * (y * moveSpeed * Time.deltaTime * sideMultiplier * forwardMultiplier));
        rb.AddForce(orientation.right * (x * moveSpeed * Time.deltaTime * sideMultiplier));
    }

    private void ResetSprintingEffects()
    {
        emission.rateOverTimeMultiplier = 0;
        if (!sprinting)
        {
            Camera cam = PlayerCamera.Instance.cam;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, defaultFOV, 5f * Time.deltaTime);
        }
    }

    private void FovEffect()
    {
        Camera cam = PlayerCamera.Instance.cam;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, sprintFOV, 5f * Time.deltaTime);
    }

    private void SpeedLines()
    {
        float angle = Vector3.Angle(rb.velocity, PlayerCamera.Instance.transform.forward);
        float angleFactor = Mathf.Max(angle, 0.1f);
        float targetRate = rb.velocity.magnitude / angleFactor * 10f;
        targetRate = Mathf.Min(targetRate, 30f);

        emission.rateOverTimeMultiplier = Mathf.Lerp(
            emission.rateOverTimeMultiplier,
            targetRate,
            Time.deltaTime * 2f
        );
    }

    private void Jump()
    {
        if (grounded || surfing)
        {
            stamina.UseJumpStamina();

            Vector3 velocity = rb.velocity;
            rb.AddForce(Vector3.up * (jumpForce * 1.5f));
            rb.AddForce(normalVector * (jumpForce * 0.5f));

            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(velocity.x, 0f, velocity.z);
            else if (rb.velocity.y > 0f)
                rb.velocity = new Vector3(velocity.x, rb.velocity.y / 2f, velocity.z);
        }
    }

    private void ResetJump() => readyToJump = true;

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

        const float threshold = 0.09f;
        const float multiplier = 0.07f;
        float moveSpeed = speed;
        const float runSpeed = 20f;

        if ((Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f) || (mag.x < -threshold && x > 0f) ||
            (mag.x > threshold && x < 0f))
            rb.AddForce(orientation.right * (moveSpeed * Time.deltaTime * -mag.x * multiplier));

        if ((Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f) || (mag.y < -threshold && y > 0f) ||
            (mag.y > threshold && y < 0f))
            rb.AddForce(orientation.forward * (moveSpeed * Time.deltaTime * -mag.y * multiplier));

        if (Mathf.Abs(x) < 0.05f && Mathf.Abs(y) < 0.05f)
        {
            if (Mathf.Abs(rb.velocity.x) < threshold) rb.velocity = new Vector3(0, rb.velocity.y, 0);
            if (Mathf.Abs(rb.velocity.z) < threshold) rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
        }

        if (new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude > runSpeed)
        {
            float vertical = rb.velocity.y;
            Vector3 clamped = new Vector3(rb.velocity.x, 0f, rb.velocity.z).normalized * runSpeed;
            rb.velocity = new Vector3(clamped.x, vertical, clamped.z);
        }
    }

    public Vector2 FindVelRelativeToLook()
    {
        float currentY = orientation.eulerAngles.y;
        float targetY = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;
        float deltaAngle = Mathf.DeltaAngle(currentY, targetY);
        float sideAngle = 90f - deltaAngle;
        float mag = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;

        return new Vector2(
            y: mag * Mathf.Cos(deltaAngle * Mathf.Deg2Rad),
            x: mag * Mathf.Cos(sideAngle * Mathf.Deg2Rad)
        );
    }

    private bool IsFloor(Vector3 v) => Vector3.Angle(Vector3.up, v) < maxSlopeAngle;

    private void OnCollisionEnter(Collision other)
    {
        int layer = other.gameObject.layer;
        Vector3 normal = other.contacts[0].normal;
        if ((int)whatIsGround != ((int)whatIsGround | (1 << layer)))
            return;

        if (IsFloor(normal) && PlayerCamera.Instance != null)
            PlayerCamera.Instance.BobOnce(new Vector3(0f, fallSpeed, 0f));
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (IsFloor(contact.normal))
            {
                grounded = true;
                normalVector = contact.normal;
                return;
            }
        }
    }

    private void OnCollisionExit() => grounded = false;
}