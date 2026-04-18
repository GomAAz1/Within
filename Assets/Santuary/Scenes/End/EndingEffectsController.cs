using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // لازم تتأكد إن الـ URP Package موجودة
using System.Collections;

public class EndingEffectsController : MonoBehaviour
{
    public Volume globalVolume;
    private Vignette vignette;
    private ChromaticAberration chromatic;

    void Awake()
    {
        // بنسحب المكونات أول ما اللعبة تبدأ
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out vignette);
            globalVolume.profile.TryGet(out chromatic);
        }
    }

    void Start()
    {
        // بنبدأ بشاشة سودة
        if (vignette != null)
            vignette.intensity.value = 1f;

        // اختياري: لو عايز تشغل الرمشة أول ما اللعبة تبدأ فورا
        // StartBlinking(); 
    }

    // وظيفة الرمشة
    public void StartBlinking()
    {
        StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkRoutine()
    {
        for (int i = 0; i < 3; i++)
        {
            if (vignette != null) vignette.intensity.value = 1f;
            yield return new WaitForSeconds(0.3f);
            if (vignette != null) vignette.intensity.value = 0.3f;
            yield return new WaitForSeconds(0.3f);
        }
        if (vignette != null) vignette.intensity.value = 0.2f;
    }

    // وظيفة الزغللة
    public void StartDizzyEffect()
    {
        StartCoroutine(DizzyRoutine());
    }

    IEnumerator DizzyRoutine()
    {
        if (chromatic != null)
        {
            chromatic.intensity.value = 1f;
            float timer = 1f;
            while (timer > 0)
            {
                timer -= Time.deltaTime * 0.4f;
                chromatic.intensity.value = Mathf.Max(0, timer);
                yield return null;
            }
        }
    }
}