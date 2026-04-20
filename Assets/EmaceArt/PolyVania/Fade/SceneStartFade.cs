using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneStartFade : MonoBehaviour
{
    public Image fadePanel;        // اسحب الصورة السوداء هنا
    public float fadeDuration = 2f; // مدة تفتيح الشاشة (ثانيتين مثلاً)

    void Start()
    {
        if (fadePanel != null)
        {
            // بنبدأ والشاشة سودة تماماً (Alpha = 1)
            fadePanel.color = new Color(0, 0, 0, 1);

            // نبدأ عملية التفتيح أوتوماتيك أول ما السين يفتح
            StartCoroutine(FadeFromBlack());
        }
    }

    IEnumerator FadeFromBlack()
    {
        float timer = 0;

        // استراحة بسيطة جداً (نص ثانية) عشان التحميل يخلص بهدوء
        yield return new WaitForSeconds(0.5f);

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // بنقلل الشفافية تدريجياً من 1 لـ 0 (Fade Out للسواد)
            float alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // في الآخر بنخفي الاوبجكت خالص عشان م يعطلش أي ضغطات ماوس في اللعبة
        fadePanel.gameObject.SetActive(false);
        Debug.Log("المرحلة بدأت بنجاح!");
    }
}