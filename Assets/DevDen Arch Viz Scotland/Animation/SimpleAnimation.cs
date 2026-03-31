using UnityEngine;

public class SimpleAnimation : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 7f;
    public float jumpHeight = 8f;
    public float gravity = 20f;

    [Header("Mouse Settings")]
    public Transform playerCamera;
    public float mouseSensitivity = 200f;
    public float lookXLimit = 45f;

    private CharacterController controller;
    private Animator anim;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. الماوس
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(Vector3.up * mouseX);


        // 2. الحركة (مفتوحة دايماً)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // بنحسب الحركة على الأرض (X و Z)
        Vector3 targetMove = (forward * v) + (right * h);
        targetMove *= currentSpeed;

        moveDirection.x = targetMove.x;
        moveDirection.z = targetMove.z;


        // 3. النط والجاذبية (لو على الأرض بس)
        if (controller.isGrounded)
        {
            moveDirection.y = -0.5f; // تثبيت اللاعب على الأرض

            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpHeight;
                anim.SetTrigger("JumpTrigger");
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);


        // 4. الأنيميشن
        float inputMagnitude = new Vector2(h, v).magnitude;
        float animSpeed = isRunning && inputMagnitude > 0 ? 1.0f : inputMagnitude;
        anim.SetFloat("Speed", animSpeed);


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}