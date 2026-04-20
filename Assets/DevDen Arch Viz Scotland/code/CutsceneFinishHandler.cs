using UnityEngine;
using UnityEngine.Playables; // مهم جداً عشان يكلم التايم لاين
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class CutsceneFinishHandler : MonoBehaviour
{
    public PlayableDirector director; // اسحب الاوبجكت اللي عليه التايم لاين هنا
    public Image fadeImage;           // اسحب الصورة السوداء هنا
    public string nextSceneName;      // اسم المرحلة اللي هتبدأ بعد الكت سين

    private bool hasStartedTransition = false;

    void OnEnable()
    {
        // بنقول للكود: أول ما التايم لاين يخلص (يتوقف)، نادي الدالة دي
        director.stopped += OnTimelineStopped;
    }

    void OnDisable()
    {
        director.stopped -= OnTimelineStopped;
    }

    void OnTimelineStopped(PlayableDirector aDirector)
    {
        if (!hasStartedTransition)
        {
            StartCoroutine(FadeAndLoadNextScene());
        }
    }

    IEnumerator FadeAndLoadNextScene()
    {
        hasStartedTransition = true;
        float duration = 2f; // مدة التسويد
        float timer = 0;

        // 1. تسويد الشاشة تدريجياً
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // 2. انتظار ثانية في السواد (للمود)
        yield return new WaitForSeconds(1f);

        // 3. الانتقال للمرحلة الحقيقية
        SceneManager.LoadScene(nextSceneName);
    }
}