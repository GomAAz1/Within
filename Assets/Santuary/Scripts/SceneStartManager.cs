using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneStartManager : MonoBehaviour
{
    public Image fadeImage; // اسحب الصورة السوداء هنا
    public float fadeDuration = 2f; // مدة التفتيح (ثانيتين مثلاً)

    void Start()
    {
        if (fadeImage != null)
        {
            // بنبدأ والشاشة سودة تماماً
            fadeImage.color = new Color(0, 0, 0, 1);
            // بنبدأ عملية التفتيح
            StartCoroutine(FadeFromBlack());
        }
    }

    IEnumerator FadeFromBlack()
    {
        float timer = 0;

        // بنستنى نص ثانية بس عشان التحميل يخلص بهدوء
        yield return new WaitForSeconds(0.5f);

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // بنقلل الشفافية تدريجياً من 1 لـ 0
            float alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // في الآخر بنخفي الصورة خالص عشان م تعطلش الماوس
        fadeImage.gameObject.SetActive(false);
        Debug.Log("بدأت المرحلة بسلام!");
    }
}