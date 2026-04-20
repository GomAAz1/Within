using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class CutsceneFinishHandler : MonoBehaviour
{
    [Header("References")]
    public PlayableDirector director; // اسحب اوبجكت التايم لاين هنا
    public Image fadeImage;           // اسحب الصورة السوداء هنا
    public string nextSceneName;      // اسم المرحلة الجاية

    [Header("Timing Settings")]
    public float startFadeDuration = 2f; // وقت تفتيح الشاشة في الأول
    public float endFadeDuration = 2f;   // وقت تسويد الشاشة في الآخر

    private bool isEnding = false;

    void Start()
    {
        // 1. نبدأ والشاشة سودة تماماً
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.color = new Color(0, 0, 0, 1);
            StartCoroutine(StartSequence());
        }
    }

    // --- مشهد البداية: تفتيح ثم تشغيل التايم لاين ---
    IEnumerator StartSequence()
    {
        yield return new WaitForSeconds(0.5f); // استراحة بسيطة

        float timer = 0;
        while (timer < startFadeDuration)
        {
            timer += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, timer / startFadeDuration));
            yield return null;
        }

        // دلوقت بس نفتح التايم لاين
        if (director != null) director.Play();
        Debug.Log("الستارة اتفتحت.. ابدأ الفيلم!");
    }

    // --- مشهد النهاية: لما التايم لاين يخلص ---
    void OnEnable() { if (director != null) director.stopped += OnTimelineStopped; }
    void OnDisable() { if (director != null) director.stopped -= OnTimelineStopped; }

    void OnTimelineStopped(PlayableDirector aDirector)
    {
        if (!isEnding) StartCoroutine(EndSequence());
    }

    IEnumerator EndSequence()
    {
        isEnding = true;
        float timer = 0;

        // تسويد الشاشة تاني
        while (timer < endFadeDuration)
        {
            timer += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, timer / endFadeDuration));
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nextSceneName);
    }
}