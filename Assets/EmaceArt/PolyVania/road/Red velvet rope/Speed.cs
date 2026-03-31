using UnityEngine;

public class MovingBridge : MonoBehaviour
{
    [Header("إعدادات الحركة")]
    public float speed = 2.0f;      // سرعة الحركة
    public float distance = 3.0f;   // المسافة التي سيتحركها يميناً ويساراً

    private Vector3 startPos;

    void Start()
    {
        // حفظ موقع الجسر الأصلي عند تشغيل اللعبة
        startPos = transform.position;
    }

    void Update()
    {
        // حساب الإزاحة باستخدام دالة Sin لتعطي حركة ناعمة ذهاباً وإياباً
        float movement = Mathf.Sin(Time.time * speed) * distance;

        // تطبيق الحركة على المحور X (يمين ويسار)
        // يمكنك تغيير startPos.x إلى startPos.z إذا كان اتجاه الجسر مختلفاً
        transform.position = new Vector3(startPos.x + movement, startPos.y, startPos.z);
    }
}