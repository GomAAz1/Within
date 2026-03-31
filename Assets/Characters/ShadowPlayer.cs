using UnityEngine;

public class ShadowPlayer   : MonoBehaviour
{
    [Header("Two Players Setup")]
    public Transform realPlayer;   // اسحب اللاعب الحقيقي هنا
    public Transform shadowPlayer; // اسحب اللاعب الضل هنا
    public Transform playerCamera; // الكاميرا
    public bool isShadowActive = false; // هل نتحكم في الضل دلوقت؟

    [Header("Movement Speeds")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 7.5f;

    [Header("Settings")]
    public float mouseSensitivity = 200f;
    public LayerMask groundMask;

    private Rigidbody realRB, shadowRB;
    private Animator realAnim, shadowAnim;
    private float xRotation = 0f;

    void Start()
    {
        realRB = realPlayer.GetComponent<Rigidbody>();
        shadowRB = shadowPlayer.GetComponent<Rigidbody>();
        realAnim = realPlayer.GetComponent<Animator>();
        shadowAnim = shadowPlayer.GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMouseLook();

        // زرار التبديل (Tab)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchPlayer();
        }

        // النط (بيشتغل للي أنت بتتحكم فيه دلوقت)
        if (Input.GetButtonDown("Jump"))
        {
            JumpLogic();
        }

        HandleAnimations();
    }

    void FixedUpdate()
    {
        MoveLogic();
    }

    void MoveLogic()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && v > 0.1f;
        float speed = isRunning ? runSpeed : walkSpeed;

        // --- حساب الحركة للاعب الحقيقي ---
        // لو إحنا مش في وضع الضل: يمشي عادي. لو في وضع الضل: يمشي عكس الـ H
        float realH = isShadowActive ? -h : h;
        Vector3 realDir = (realPlayer.forward * v + realPlayer.right * realH).normalized;
        realRB.linearVelocity = new Vector3(realDir.x * speed, realRB.linearVelocity.y, realDir.z * speed);

        // --- حساب الحركة للاعب الضل ---
        // لو إحنا في وضع الضل: يمشي عادي. لو في وضع الحقيقة: يمشي عكس الـ H
        float shadowH = isShadowActive ? h : -h;
        Vector3 shadowDir = (shadowPlayer.forward * v + shadowPlayer.right * shadowH).normalized;
        shadowRB.linearVelocity = new Vector3(shadowDir.x * speed, shadowRB.linearVelocity.y, shadowDir.z * speed);
    }

    void SwitchPlayer()
    {
        isShadowActive = !isShadowActive;
        // هنا الكاميرا هتنقل مكانها (هنطور دي دلوقت)
        Debug.Log(isShadowActive ? "التحكم في الضل" : "التحكم في الحقيقي");
    }

    void JumpLogic()
    {
        // بيفحص مين اللي واقف على الأرض وينططه
        if (!isShadowActive && IsGrounded(realPlayer))
        {
            realRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            realAnim.Play("Jump_Up", 0, 0);
        }
        else if (isShadowActive && IsGrounded(shadowPlayer))
        {
            shadowRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            shadowAnim.Play("Jump_Up", 0, 0);
        }
    }

    bool IsGrounded(Transform t)
    {
        return Physics.Raycast(t.position + Vector3.up * 0.1f, Vector3.down, 0.6f, groundMask);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // الماوس بيلف الشخصيتين مع بعض دايماً
        realPlayer.Rotate(Vector3.up * mouseX);
        shadowPlayer.Rotate(Vector3.up * mouseX);
    }

    void HandleAnimations()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool moving = (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f);

        // ربط الأنيميشن للاثنين (بالأسماء اللي كانت شغالة معاك)
        UpdateAnim(realAnim, h, v, moving);
        UpdateAnim(shadowAnim, h, v, moving);
    }

    void UpdateAnim(Animator a, float h, float v, bool moving)
    {
        a.SetBool("isMoving", moving);
        a.SetBool("iswalking", moving && !Input.GetKey(KeyCode.LeftShift));
        a.SetBool("isrunning", moving && Input.GetKey(KeyCode.LeftShift));
        // ملحوظة: الاتجاهات في الضل هتشتغل برضه أوتوماتيك لأننا بنبعت الـ h و v
    }
}