using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float sprintSpeed = 9f;
    public float jumpForce = 7f;
    public float wallRunSpeed = 7f;
    public float gravityMultiplier = 2f; 

    [Header("Mouse Settings")]
    public Transform playerCamera;
    public float mouseSensitivity = 200f;
    public float maxLookUp = 75f;
    public float maxLookDown = -75f;

    [Header("Wall Settings")]
    public float wallCheckDistance = 1f;
    public float wallJumpForce = 7f;

    private Rigidbody rb;
    private bool isGrounded;
    private bool isWallRight;
    private bool isWallLeft;
    private float xRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // لمنع دوران Rigidbody طبيعي
        Physics.gravity *= gravityMultiplier;

        if (playerCamera == null)
        {
            playerCamera = Camera.main.transform;
        }
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        CheckWalls();
        HandleWallRunAndJump();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, maxLookDown, maxLookUp);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");

        Vector3 move = transform.forward * moveZ + transform.right * moveX;

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;

        // اذا على wallRun, speed يكون wallRunSpeed فقط إذا يتحرك جانبياً
        if (!isGrounded && (isWallLeft || isWallRight))
        {
            // نسمح فقط للحركة على طول الحائط (الجوانب)
            if ((isWallRight && moveX > 0) || (isWallLeft && moveX < 0))
            {
                currentSpeed = wallRunSpeed;
            }
        }

        rb.linearVelocity = new Vector3(move.x * currentSpeed, rb.linearVelocity.y, move.z * currentSpeed);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void CheckWalls()
    {
        // فقط الجانبين
        isWallRight = Physics.Raycast(transform.position, transform.right, wallCheckDistance);
        isWallLeft = Physics.Raycast(transform.position, -transform.right, wallCheckDistance);
    }

    void HandleWallRunAndJump()
    {
        if (!isGrounded && (isWallLeft || isWallRight))
        {
            // تثبيت اللاعب على ارتفاعه للحفاظ على wall run
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // دفع للأمام فقط إذا الحركة على طول الحائط
            if ((isWallRight && Input.GetAxis("Horizontal") > 0) || (isWallLeft && Input.GetAxis("Horizontal") < 0))
            {
                rb.AddForce(transform.forward * wallRunSpeed, ForceMode.Force);
            }

            // Wall Jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 jumpDir = Vector3.up;
                if (isWallRight) jumpDir += -transform.right;
                else if (isWallLeft) jumpDir += transform.right;

                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                rb.AddForce(jumpDir * wallJumpForce, ForceMode.Impulse);
            }

            // نزول تلقائي عند نهاية الحائط
            if (!isWallRight && !isWallLeft)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -2f, rb.linearVelocity.z);
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}