using UnityEngine;
using System;

// USED EXISTING CODE FROM SOME OF MY OLDER CODE FOR THIS MOVEMENT SCRIPT,
// still took the time to code it the way i wanted it to work

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    
    [Header("Movement properties")]
    [SerializeField] private float speed = 2000f;
    [SerializeField] private float jumpForce = 300f;
    [SerializeField] private float groundDrag = 1;
    [SerializeField] private float airMultiplier = 0.6f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private LayerMask whatIsGround;
    
    private bool readyToJump = true;
    private bool grounded;
    
    private bool wallRunning;
    private bool surfing;
    private float jumpCooldown = 0.25f;
    private Vector3 normalVector;
    private Vector3 wallNormalVector = Vector3.up;
    private float maxSlopeAngle = 35f;

    [Header("Input")] 
    private float x;
    private float y;
    private bool jumping;
    
    public Transform orientation;
    public Transform camTransform;
    [Header("Mouse Look")]
    public float sensitivity = 50f;
    public float sensMultiplier = 1f;
    private float xRotation = 0f;
    private float desiredX;

    public static PlayerMovement Instance;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        
        if (orientation == null)
            orientation = transform; 
        
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (camTransform == null && Camera.main != null)
            camTransform = Camera.main.transform;
    }

    private void Start()
    {
        desiredX = orientation.localEulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        GetInput();
        HandleDrag();
        HandleLook();

        if (readyToJump && grounded && jumping)
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

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * sensMultiplier;
        
        desiredX += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (camTransform != null)
            camTransform.localRotation = Quaternion.Euler(xRotation, desiredX, 0f);
        
        orientation.Rotate(Vector3.up * mouseX);
    }

    private void HandleDrag()
    {
        rb.drag = grounded ? groundDrag : 0f;
    }

    private void GetInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
    }

    private void Movement()
    {
        rb.AddForce(Vector3.down * Time.deltaTime * 12.5f);
        
        Vector2 mag = FindVelRelativeToLook();
        float xVelLook = mag.x;
        float yVelLook = mag.y;
        
        CounterMovement(x, y, mag);
        
        float maxSpeed = 20f;
        float moveSpeed = speed;

        if (x > 0f && xVelLook > maxSpeed) x = 0f;
        if (x < 0f && xVelLook < -maxSpeed) x = 0f;
        if (y > 0f && yVelLook > maxSpeed) y = 0f;
        if (y < 0f && yVelLook < -maxSpeed) y = 0f;

        float forwardMultiplier = grounded ? 1f : airMultiplier;
        float sideMultiplier = grounded ? 1f : airMultiplier;

        if (wallRunning)
        {
            forwardMultiplier = 0.3f;
            sideMultiplier = 0.3f;
        }
        if (surfing)
        {
            forwardMultiplier = 0.7f;
            sideMultiplier = 0.3f;
        }
        
        rb.AddForce(orientation.forward * (y * moveSpeed * Time.deltaTime * sideMultiplier * forwardMultiplier));
        rb.AddForce(orientation.right * (x * moveSpeed * Time.deltaTime * sideMultiplier));
    }

    private void Jump()
    {
        if (grounded || wallRunning || surfing)
        {
            Vector3 velocity = rb.velocity;
            
            rb.AddForce(Vector3.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);
            
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(velocity.x, 0f, velocity.z);
            else if (rb.velocity.y > 0f)
                rb.velocity = new Vector3(velocity.x, rb.velocity.y / 2f, velocity.z);

            if (wallRunning)
                rb.AddForce(wallNormalVector * jumpForce * 3f);
        }
    }

    private void ResetJump() => readyToJump = true;

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

        float threshold = 0.09f;
        float multiplier = 0.07f;
        float moveSpeed = speed;
        float runSpeed = 20f;

        if ((Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f) || (mag.x < -threshold && x > 0f) || (mag.x > threshold && x < 0f))
            rb.AddForce(moveSpeed * orientation.right * Time.deltaTime * -mag.x * multiplier);

        if ((Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f) || (mag.y < -threshold && y > 0f) || (mag.y > threshold && y < 0f))
            rb.AddForce(moveSpeed * orientation.forward * Time.deltaTime * -mag.y * multiplier);

        if (Mathf.Abs(x) < 0.05f && Mathf.Abs(y) < 0.05f)
        {
            if (Mathf.Abs(rb.velocity.x) < threshold) rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
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

    private void OnCollisionExit(Collision collision) => grounded = false;
}
