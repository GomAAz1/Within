using UnityEngine;

public class PlayerAnimationPlayer : MonoBehaviour
{
    [Header("Movement Speeds")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 7f;

    [Header("Detection Settings")]
    public float groundCheckDistance = 0.6f;
    public LayerMask groundMask;

    [Header("References")]
    public Transform playerCamera;
    public float mouseSensitivity = 200f;

    private Rigidbody rb;
    private Animator anim;
    private bool isGrounded;
    private float xRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        rb.freezeRotation = true;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMouseLook();

        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance, groundMask);
        anim.SetBool("isGrounded", isGrounded);

        // --- تصليح النط (الأمر المباشر) ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);

            float v = Input.GetAxis("Vertical");
            bool isRunning = Input.GetKey(KeyCode.LeftShift) && v > 0.1f;

            // السر هنا: بنادي اسم المربع الرمادي مباشرة عشان نتخطى أي غلط في الأسهم
            if (isRunning) anim.CrossFadeInFixedTime("Jump_Run", 0.1f);
            else anim.CrossFadeInFixedTime("Jump_Up", 0.1f);
        }

        HandleMovementAndAnimation();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 moveDir = (transform.forward * v + transform.right * h).normalized;
        bool isRunningInput = Input.GetKey(KeyCode.LeftShift) && (Mathf.Abs(h) > 0.1f || v > 0.1f);
        float currentSpeed = isRunningInput ? runSpeed : walkSpeed;
        rb.linearVelocity = new Vector3(moveDir.x * currentSpeed, rb.linearVelocity.y, moveDir.z * currentSpeed);
    }

    void HandleMovementAndAnimation()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool moving = (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f);
        bool running = moving && Input.GetKey(KeyCode.LeftShift) && v > 0.1f;

        // لو في الهوا.. "اسكت" وما تبعتش أي أوامر مشي عشان النطة تبان كاملة
        if (!isGrounded) return;

        anim.SetBool("isMoving", moving);
        anim.SetBool("iswalking", false);
        anim.SetBool("isrunning", false);
        anim.SetBool("isWalkingBack", false);
        anim.SetBool("isWalkingRight", false);
        anim.SetBool("isWalkingLeft", false);

        if (moving)
        {
            if (running) anim.SetBool("isrunning", true);
            else if (v < -0.1f) anim.SetBool("isWalkingBack", true);
            else if (h > 0.1f) anim.SetBool("isWalkingRight", true);
            else if (h < -0.1f) anim.SetBool("isWalkingLeft", true);
            else anim.SetBool("iswalking", true);
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}