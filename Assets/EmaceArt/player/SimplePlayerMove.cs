using UnityEngine;

public class SimplePlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 8f;
    public float gravity = 30f;

    private CharacterController cc;
    private Animator anim;
    private float yVelocity = -2f;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool run = Input.GetKey(KeyCode.LeftShift);
        bool jump = Input.GetButtonDown("Jump");

        // اتجاه الحركة
        float speed = run ? runSpeed : walkSpeed;
        Vector3 move = (transform.forward * v + transform.right * h).normalized * speed;

        // الفيزياء والجاذبية
        if (cc.isGrounded)
        {
            yVelocity = -2f;
            if (jump)
            {
                yVelocity = jumpForce;
                anim.CrossFadeInFixedTime(run ? "Jump_Run" : "Jump_Up", 0.1f);
            }
        }
        else yVelocity -= gravity * Time.deltaTime;

        move.y = yVelocity;
        cc.Move(move * Time.deltaTime);

        // تحديث الأنيميشن
        UpdateAnimator(h, v, run);
    }

    void UpdateAnimator(float h, float v, bool run)
    {
        bool moving = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;
        anim.SetBool("isGrounded", cc.isGrounded);
        anim.SetBool("isMoving", moving);
        anim.SetBool("iswalking", moving && !run);
        anim.SetBool("isrunning", moving && run);
        anim.SetBool("isWalkingBack", moving && v < -0.1f);

        if (Mathf.Abs(h) > Mathf.Abs(v))
        {
            anim.SetBool("isWalkingRight", h > 0.1f);
            anim.SetBool("isWalkingLeft", h < -0.1f);
        }
        else
        {
            anim.SetBool("isWalkingRight", false);
            anim.SetBool("isWalkingLeft", false);
        }
    }
}