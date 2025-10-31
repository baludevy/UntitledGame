using UnityEngine;
using System;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float walkSpeed = 1250f;
    [SerializeField] private float sprintSpeed = 3000f;
    [SerializeField] private float jumpForce = 350f;
    [SerializeField] private float groundDrag = 3f;
    [SerializeField] private float slideDrag = 0.5f;
    [SerializeField] private float airMultiplier = 0.7f;
    [SerializeField] private LayerMask whatIsGround;

    private float startWalkSpeed;
    private float startSprintSpeed;

    private bool readyToJump = true;
    private bool grounded;
    private bool surfing;
    private bool sliding;
    private bool canSlide = true;
    private const float jumpCooldown = 0.25f;
    private Vector3 normalVector;
    private const float maxSlopeAngle = 35f;
    private float fallSpeed;

    [SerializeField] private float slideForce = 400f;

    [SerializeField] private float slideCounterMovement = 0.2f;
    [SerializeField] private Vector3 crouchScale = new Vector3(2f, 2.25f, 2f);
    [SerializeField] private float slideCooldown = 0.2f;
    private Vector3 playerScale;

    private float xInput;
    private float yInput;
    private bool jumping;
    private bool sprinting;
    private bool crouching;

    public float sensitivity = 50f;
    public float sensMultiplier = 1f;
    private float xRotation;
    private float desiredX;
    public Transform orientation;
    public Transform camTransform;
    public bool canLook = true;

    public float defaultFOV = 90f;
    public float sprintFOV = 100f;

    private float walkBobTimer;
    private readonly float bobSpeed = 8f;
    private const float bobAmount = 0.8f;

    [SerializeField] private float footstepInterval = 0.4f;
    private float footstepTimer;

    public ParticleSystem ps;
    private ParticleSystem.EmissionModule emission;
    private float drag;

    private PlayerStamina stamina;

    public static PlayerMovement Instance;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        startWalkSpeed = walkSpeed;
        startSprintSpeed = sprintSpeed;
    }

    private void Start()
    {
        playerScale = transform.localScale;
        desiredX = orientation.localEulerAngles.y;

        CursorManager.LockCursor();

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
    }

    private void FixedUpdate()
    {
        Movement();
    }

    #region input

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

    private void GetInput()
    {
        if (PlayerUIManager.Instance.GetInventoryState())
        {
            xInput = 0;
            yInput = 0;
            jumping = false;
            sprinting = false;
            return;
        }

        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (crouching)
        {
            xInput = 0;
            yInput = 0;
        }

        jumping = Input.GetButton("Jump");
        sprinting = Input.GetButton("Sprint");

        if (readyToJump && grounded && jumping && stamina.GetStamina() >= stamina.GetJumpStaminaLoss())
        {
            readyToJump = false;
            Invoke(nameof(ResetJump), jumpCooldown);
            Jump();
        }

        if (Input.GetKey(KeyCode.LeftControl) && (!crouching || !sliding)) StartSlide();
        if (Input.GetKeyUp(KeyCode.LeftControl) && crouching) StopSlide();
    }

    #endregion

    #region movement

    private void Movement()
    {
        drag = crouching ? slideDrag : groundDrag;
        rb.AddForce(Vector3.down * (Time.deltaTime * 12.5f));

        Vector2 mag = FindVelRelativeToLook();
        //calculates velocity relative to where player is looking

        float xVelLook = mag.x;
        float yVelLook = mag.y;

        CounterMovement(xInput, yInput, mag);

        float maxSpeed = 25f;
        float moveSpeed = walkSpeed;

        bool isMoving = xInput != 0 || yInput != 0;
        bool isSprinting = sprinting && isMoving && stamina.GetStamina() > 0f;

        if (isSprinting)
        {
            moveSpeed = sprintSpeed;
            stamina.UseStamina(stamina.GetStaminaLoss() * Time.deltaTime);
            SpeedLines();
            FovEffect();
        }
        else
        {
            ResetSprintingEffects();
        }

        if (xInput > 0f && xVelLook > maxSpeed) xInput = 0f;
        if (xInput < 0f && xVelLook < -maxSpeed) xInput = 0f;
        if (yInput > 0f && yVelLook > maxSpeed) yInput = 0f;
        if (yInput < 0f && yVelLook < -maxSpeed) yInput = 0f;

        float forwardMultiplier = grounded ? 1f : airMultiplier;
        float sideMultiplier = grounded ? 1f : airMultiplier;

        if (surfing)
        {
            forwardMultiplier = 0.7f;
            sideMultiplier = 0.3f;
        }

        if (sliding && grounded)
        {
            //slide movement physics and counterforce
            rb.AddForce(-rb.velocity.normalized * (moveSpeed * Time.deltaTime * slideCounterMovement));
            rb.AddForce(Vector3.down * (Time.deltaTime * 3000f));
            return;
        }

        rb.AddForce(orientation.forward * (yInput * moveSpeed * Time.deltaTime * sideMultiplier * forwardMultiplier));
        rb.AddForce(orientation.right * (xInput * moveSpeed * Time.deltaTime * sideMultiplier));
    }

    private void Jump()
    {
        if (grounded || surfing)
        {
            stamina.UseJumpStamina();
            Vector3 velocity = rb.velocity;
            rb.AddForce(Vector3.up * (jumpForce * 1.5f));
            rb.AddForce(normalVector * (jumpForce * 0.5f));
            //adjust y velocity to smooth jump start
            if (rb.velocity.y < 0.5f) rb.velocity = new Vector3(velocity.x, 0f, velocity.z);
            else if (rb.velocity.y > 0f) rb.velocity = new Vector3(velocity.x, rb.velocity.y / 2f, velocity.z);
        }
    }

    private void ResetJump() => readyToJump = true;

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;
        const float threshold = 0.09f;
        const float multiplier = 0.07f;
        float moveSpeed = walkSpeed;
        const float runSpeed = 20f;

        if (crouching)
        {
            rb.AddForce(-rb.velocity.normalized * (moveSpeed * Time.deltaTime * slideCounterMovement));
            return;
        }

        //countermovement when stopping or changing direction
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

        //cap run speed
        if (new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude > runSpeed)
        {
            float vertical = rb.velocity.y;
            Vector3 clamped = new Vector3(rb.velocity.x, 0f, rb.velocity.z).normalized * runSpeed;
            rb.velocity = new Vector3(clamped.x, vertical, clamped.z);
        }
    }

    public Vector2 FindVelRelativeToLook()
    {
        //finds player velocity relative to camera orientation
        float currentY = orientation.eulerAngles.y;
        float targetY = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;
        float deltaAngle = Mathf.DeltaAngle(currentY, targetY);
        float sideAngle = 90f - deltaAngle;
        float mag = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;
        return new Vector2(y: mag * Mathf.Cos(deltaAngle * Mathf.Deg2Rad),
            x: mag * Mathf.Cos(sideAngle * Mathf.Deg2Rad));
    }

    private void HandleDrag()
    {
        rb.drag = grounded ? drag : 0f;
    }

    #endregion

    #region crouching/sliding

    private void StartSlide()
    {
        if (!grounded || !canSlide || crouching) return;

        crouching = true;
        transform.localScale = crouchScale;

        Vector3 pos = transform.position;
        pos.y -= (playerScale.y - crouchScale.y) * 0.5f;
        rb.MovePosition(pos);

        float horizontalSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;

        if (horizontalSpeed > 2f)
        {
            rb.AddForce(orientation.forward * slideForce, ForceMode.Impulse);
            sliding = true;
            canSlide = false;
            Invoke(nameof(ResetSlideCooldown), slideCooldown);
        }
    }

    private void StopSlide()
    {
        if (!crouching) return;

        crouching = false;
        sliding = false;
        transform.localScale = playerScale;

        Vector3 pos = transform.position;
        pos.y += (playerScale.y - crouchScale.y) * 0.5f;
        rb.MovePosition(pos);
    }


    private void ResetSlideCooldown()
    {
        canSlide = true;
    }

    #endregion

    #region juice

    private void WalkBob()
    {
        if (!grounded) return;
        Vector3 horizontalVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        float moveSpeed = horizontalVel.magnitude;
        if (moveSpeed > 1f && PlayerCamera.Instance != null)
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
                footstepTimer = footstepInterval;
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
        //smooth transition between fov when sprinting
        Camera cam = PlayerCamera.Instance.cam;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, sprintFOV, 5f * Time.deltaTime);
    }

    private void SpeedLines()
    {
        //speed lines based on velocity and camera angle
        float angle = Vector3.Angle(rb.velocity, PlayerCamera.Instance.transform.forward);
        float angleFactor = Mathf.Max(angle, 0.1f);
        float targetRate = rb.velocity.magnitude / angleFactor * 10f;
        targetRate = Mathf.Min(targetRate, 30f);
        emission.rateOverTimeMultiplier = Mathf.Lerp(emission.rateOverTimeMultiplier, targetRate, Time.deltaTime * 2f);
    }

    #endregion

    private bool IsFloor(Vector3 v) => Vector3.Angle(Vector3.up, v) < maxSlopeAngle;

    private void OnCollisionEnter(Collision other)
    {
        OnPlayerContact();
        int layer = other.gameObject.layer;
        Vector3 normal = other.contacts[0].normal;
        if (IsFloor(normal)) OnPlayerLanded();
        if (whatIsGround != (whatIsGround | (1 << layer))) return;
        if (IsFloor(normal) && PlayerCamera.Instance != null)
            PlayerCamera.Instance.BobOnce(new Vector3(0f, fallSpeed, 0f));
    }

    private void OnPlayerLanded()
    {
        AbilityController.Instance.OnPlayerLanded();
    }

    private void OnPlayerContact()
    {
        AbilityController.Instance.OnPlayerContact();
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


    public void SetCanLook(bool look) => canLook = look;
    public bool GetCanLook() => canLook;
    public Rigidbody GetRigidbody() => rb;

    public float GetWalkSpeed() => walkSpeed;
    public float GetSprintSpeed() => sprintSpeed;

    public void ChangeWalkSpeed(float newWalkSpeed, float time)
    {
        StartCoroutine(ChangeWalkSpeedTemporarily(newWalkSpeed, time));
    }

    private IEnumerator ChangeWalkSpeedTemporarily(float newWalkSpeed, float time)
    {
        float originalSpeed = walkSpeed;
        walkSpeed = newWalkSpeed;
        yield return new WaitForSeconds(time);
        walkSpeed = originalSpeed;
    }

    public void ChangeWalkSpeed(float newWalkSpeed) => walkSpeed = newWalkSpeed;

    public void ResetWalkSpeed() => walkSpeed = startWalkSpeed;

    public void ChangeSprintSpeed(float newSprintSpeed, float time)
    {
        StartCoroutine(ChangeSprintSpeedTemporarily(newSprintSpeed, time));
    }

    private IEnumerator ChangeSprintSpeedTemporarily(float newSprintSpeed, float time)
    {
        float originalSpeed = sprintSpeed;
        sprintSpeed = newSprintSpeed;
        yield return new WaitForSeconds(time);
        sprintSpeed = originalSpeed;
    }

    public void ChangeSprintSpeed(float newSprintSpeed) => walkSpeed = newSprintSpeed;

    public void ResetSprintSpeed() => sprintSpeed = startSprintSpeed;

    public Vector2 GetInputDirection()
    {
        return new Vector2(xInput, yInput);
    }

    public bool IsGrounded() => grounded;

    public float GetDrag() => drag;
}