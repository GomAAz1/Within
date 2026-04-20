using UnityEngine;

public class StaticIdleCharacter : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (anim != null)
        {
            // 1. تصفير أي فاريابل ممكن يحرك الشخصية (زي Speed اللي عملناه)
            anim.SetFloat("Speed", 0f);
            anim.SetBool("isMoving", false);

            // 2. إجبار الأنيميتور إنه يشغل حالة الـ Idle فوراً
            // (تأكد إن اسم الحالة جوه الأنيميتور هو "idle" أو "Idle")
            anim.Play("idle");
        }
    }

    void Update()
    {
        // الكود فاضي هنا عشان مفيش WASD ولا أي حركة
    }
}