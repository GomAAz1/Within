using UnityEngine;

public class BotPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float walkSpeed = 2.5f; // سرعة الحركة الفزيائية
    public float rotationSpeed = 10f; // سرعة دوران اللاعب

    private Animator anim;
    private CharacterController controller;
    private Transform targetPoint;
    private bool movingToB = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        targetPoint = pointB; // يبدأ المسار نحو النقطة B
    }

    void Update()
    {
        if (pointA == null || pointB == null) return;

        // 1. حساب الاتجاه نحو الهدف
        Vector3 direction = targetPoint.position - transform.position;
        direction.y = 0; // لضمان عدم ميلان اللاعب

        // 2. التحرك والأنميشن
        // زدنا المسافة (0.6f) لضمان وصول سلس قبل التبديل
        if (direction.magnitude > 0.6f)
        {
            // تدوير اللاعب في اتجاه المشي (مهم جداً عشان الأنميشن ميبانش بايظ)
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // تحريك اللاعب للأمام
            Vector3 velocity = direction.normalized * walkSpeed;
            controller.Move(velocity * Time.deltaTime);

            // إرسال قيمة ثابتة للـ Animator لتشغيل المشي (Walk)
            // تأكد أن الاسم في الـ Animator هو Speed بـ S كبيرة كما في صورتك
            anim.SetFloat("Speed", 1.0f);
        }
        else
        {
            // عند الوصول للهدف، تبديل الهدف فوراً لضمان عدم التوقف
            movingToB = !movingToB;
            targetPoint = movingToB ? pointB : pointA;

            // لو عاوزه يقف ثانية قبل ما يرجع، ممكن نستخدم Coroutine، لكن حالياً هيفضل ماشي
        }
    }
}