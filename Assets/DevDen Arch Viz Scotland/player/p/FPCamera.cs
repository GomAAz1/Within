using UnityEngine;
using System.Collections.Generic;

public class FPCamera : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 8f;
    public float gravity = 40f;

    [Header("Camera Settings (Mouse)")]
    public Transform playerCamera; // اسحب الكاميرا هنا
    public float mouseSensitivity = 200f;
    private float xRotation = 0f;

    [Header("Footsteps Audio")]
    public AudioSource audioSource;
    public AudioClip[] footstepClips;
    public float walkStepInterval = 0.45f;
    public float runStepInterval = 0.3f;

    private CharacterController cc;
    private Animator anim;
    private float yVelocity = -2f;
    private float stepTimer;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        if (audioSource) audioSource.playOnAwake = false;

        // قفل الماوس وإخفاؤه
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (cc == null) return;

        // 1. حركة الماوس (دوران الرؤية)
        HandleMouseLook();

        // 2. قراءة مدخلات الحركة
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool run = Input.GetKey(KeyCode.LeftShift);
        bool jump = Input.GetButtonDown("Jump");

        // حساب اتجاه الحركة
        float speed = run ? runSpeed : walkSpeed;
        Vector3 move = (transform.forward * v + transform.right * h).normalized * speed;

        // 3. الجاذبية والنط
        if (cc.isGrounded)
        {
            yVelocity = -2f;
            if (jump)
            {
                yVelocity = jumpForce;
                if (anim) anim.CrossFadeInFixedTime(run ? "Jump_Run" : "Jump_Up", 0.1f);
            }
        }
        else
        {
            yVelocity -= gravity * Time.deltaTime;
        }

        move.y = yVelocity;
        cc.Move(move * Time.deltaTime);

        // 4. تحديث الأنيميشن والصوت
        if (anim) UpdateAnimator(h, v, run);
        HandleFootsteps(h, v, run);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // الدوران الرأسي (للكاميرا بس)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        if (playerCamera != null)
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // الدوران الأفقي (للجسم كله)
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleFootsteps(float h, float v, bool run)
    {
        bool isInputting = (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f);

        if (isInputting && cc.isGrounded)
        {
            stepTimer += Time.deltaTime;
            float interval = run ? runStepInterval : walkStepInterval;

            if (stepTimer >= interval)
            {
                if (footstepClips.Length > 0 && audioSource != null)
                {
                    audioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]);
                }
                stepTimer = 0;
            }
        }
        else
        {
            stepTimer = 0;
        }
    }

    void UpdateAnimator(float h, float v, bool run)
    {
        bool moving = (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f);
        anim.SetBool("isGrounded", cc.isGrounded);
        anim.SetBool("isMoving", moving);

        anim.SetBool("iswalking", false);
        anim.SetBool("isrunning", false);
        anim.SetBool("isWalkingBack", false);
        anim.SetBool("isWalkingRight", false);
        anim.SetBool("isWalkingLeft", false);

        if (moving)
        {
            if (Mathf.Abs(h) > Mathf.Abs(v))
            {
                if (h > 0.1f) anim.SetBool("isWalkingRight", true);
                else anim.SetBool("isWalkingLeft", true);
            }
            else
            {
                if (v < -0.1f) anim.SetBool("isWalkingBack", true);
                else if (run && v > 0.1f) anim.SetBool("isrunning", true);
                else anim.SetBool("iswalking", true);
            }
        }
    }
}