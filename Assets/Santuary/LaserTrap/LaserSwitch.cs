using UnityEngine;

public class LaserSwitch : MonoBehaviour
{
    [Header("الروابط")]
    public GameObject targetLaser;

    [Header("Colors (HDR)")]
    // نصيحة: اختار ألوان فاقعة جداً من الانسبكتور
    public Color activeGlowColor = Color.green;
    public Color originalGlowColor = Color.yellow;

    private MeshRenderer switchRenderer;
    private AudioSource switchAudio;
    private Material switchMat;

    void Start()
    {
        switchRenderer = GetComponent<MeshRenderer>();
        switchAudio = GetComponent<AudioSource>();

        if (switchRenderer != null)
        {
            switchMat = switchRenderer.material;
            // تفعيل خاصية التوهج في الماتريال برمجياً
            switchMat.EnableKeyword("_EMISSION");
            ApplyColorAndGlow(originalGlowColor);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ShadowPlayer"))
        {
            if (targetLaser != null)
            {
                bool isNowActive = !targetLaser.activeSelf;
                targetLaser.SetActive(isNowActive);

                if (switchAudio != null) switchAudio.Play();

                // تغيير التوهج واللون بناءً على الحالة
                Color targetColor = isNowActive ? originalGlowColor : activeGlowColor;
                ApplyColorAndGlow(targetColor);

                Debug.Log(isNowActive ? "الليزر رجع - توهج أصفر" : "الليزر طفى - توهج أخضر");
            }
        }
    }

    void ApplyColorAndGlow(Color clr)
    {
        if (switchMat != null)
        {
            // 1. تغيير اللون الأساسي
            switchMat.SetColor("_BaseColor", clr);

            // 2. تغيير لون التوهج (السر في الـ _EmissionColor)
            // بنضرب اللون في 2 عشان نخليه "ينور" أكتر في الضلمة
            switchMat.SetColor("_EmissionColor", clr * 2f);
        }
    }
}