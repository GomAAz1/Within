using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    private Movement movement;

    private int isWalkingHash, isRunningHash, isMovingHash, jumpHash;
    private int isWalkingBackHash, isWalkingRightHash, isWalkingLeftHash;
    private int isGroundedHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponentInParent<Movement>();

        isWalkingHash = Animator.StringToHash("iswalking");
        isRunningHash = Animator.StringToHash("isrunning");
        isMovingHash = Animator.StringToHash("isMoving");
        jumpHash = Animator.StringToHash("Jump");

        isWalkingBackHash = Animator.StringToHash("isWalkingBack");
        isWalkingRightHash = Animator.StringToHash("isWalkingRight");
        isWalkingLeftHash = Animator.StringToHash("isWalkingLeft");

        isGroundedHash = Animator.StringToHash("isGrounded");
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        bool running = Input.GetKey(KeyCode.LeftShift);

        bool isForward = moveZ > 0.1f;
        bool isBack = moveZ < -0.1f;
        bool isRight = moveX > 0.1f;
        bool isLeft = moveX < -0.1f;
        bool isMoving = isForward || isBack || isRight || isLeft;

        animator.SetBool(isWalkingHash, isForward);
        animator.SetBool(isRunningHash, isForward && running);
        animator.SetBool(isWalkingBackHash, isBack);
        animator.SetBool(isWalkingRightHash, isRight);
        animator.SetBool(isWalkingLeftHash, isLeft);
        animator.SetBool(isMovingHash, isMoving);

        if (movement != null)
        {
            animator.SetBool(isGroundedHash, movement.IsGrounded());
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger(jumpHash);
        }
    }
}