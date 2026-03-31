using UnityEngine;

public class PlayerAnimationPlayer : MonoBehaviour
{
    [Header("Movement Speeds")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 7f;

    [Header("Detection Settings")]
    public float groundCheckDistance = 0.3f; // تقليل المسافة لزيادة الدقة
    public LayerMask groundMask;

    [Header("Step Offset (تخطي العقبات)")]
    public float stepHeight = 0.5f;          // أقصى ارتفاع للدرجة (زودها لو لسه مش بيطلع)
    public float stepSmooth = 0.2f;          // سرعة الرفع
    public float detectionDistance = 0.6f;   // مسافة فحص العائق أمام اللاعب

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

        // نصيحة: تأكد أن Capsule Collider الخاص باللاعب ملوش الاحتكاك (Friction = 0)
    }

    void Update()
    {
        HandleMouseLook();

        // فحص الأرضية من أسفل القدم مباشرة
        isGrounded = Physics.CheckSphere(transform.position + Vector3.up * 0.1f, groundCheckDistance, groundMask);
        anim.SetBool("isGrounded", isGrounded);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            float v = Input.GetAxis("Vertical");
            bool isRunning = Input.GetKey(KeyCode.LeftShift) && v > 0.1f;
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

        if (moveDir.magnitude > 0.1f)
        {
            rb.linearVelocity = new Vector3(moveDir.x * currentSpeed, rb.linearVelocity.y, moveDir.z * currentSpeed);
            StepClimb(moveDir); // استدعاء دالة التسلق
        }
        else
        {
            // منع الانزلاق عند التوقف
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    // دالة احترافية لتخطى العقبات
    void StepClimb(Vector3 direction)
    {
        RaycastHit hitLower;
        // 1. فحص أسفل (عند القدم)
        if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), direction, out hitLower, detectionDistance, groundMask))
        {
            RaycastHit hitUpper;
            // 2. فحص أعلى (عند أقصى ارتفاع مسموح للدرجة)
            // لو الشعاع اللي فوق مخبطش في حاجة، معناه إن اللي قدامي عتبة مش حيطة
            if (!Physics.Raycast(transform.position + new Vector3(0, stepHeight, 0), direction, out hitUpper, detectionDistance + 0.1f, groundMask))
            {
                // 3. رفع اللاعب تدريجياً ليتخطى العتبة
                // بنضيف قوة لفوق أو بنحرك الـ Position مباشرة لضمان التخطي
                rb.position += new Vector3(0, stepSmooth, 0);

                // دفعة بسيطة للأمام عشان م يلزقش في طرف الدرجة
                rb.linearVelocity += direction * 0.1f;
            }
        }
    }

    void HandleMovementAndAnimation()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool moving = (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f);
        bool running = moving && Input.GetKey(KeyCode.LeftShift) && v > 0.1f;

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