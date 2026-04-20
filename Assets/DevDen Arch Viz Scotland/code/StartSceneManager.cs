using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class StartSceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image fadePanel;        // اسحب الصورة السوداء هنا
    public GameObject objectiveUI; // اسحب نص المهمة هنا

    [Header("Settings")]
    public float fadeDuration = 2.5f; // وقت تفتيح الشاشة
    public float waitBeforeShowTask = 0.5f; // استراحة قبل ظهور النص

    void Start()
    {
        // التأكد من الحالة الابتدائية
        if (fadePanel != null) fadePanel.color = new Color(0, 0, 0, 1);
        if (objectiveUI != null) objectiveUI.SetActive(false);

        // بدء سحر البداية
        StartCoroutine(BeginLevelSequence());
    }

    IEnumerator BeginLevelSequence()
    {
        float timer = 0;

        // 1. تفتيح الشاشة من السواد (Fade Out the blackness)
        yield return new WaitForSeconds(0.5f); // استراحة بسيطة عشان التحميل يخلص

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadePanel.gameObject.SetActive(false); // اخفيها خالص عشان م تعطلش الماوس

        // 2. إظهار المهمة
        yield return new WaitForSeconds(waitBeforeShowTask);
        if (objectiveUI != null) objectiveUI.SetActive(true);

        Debug.Log("بدأت المرحلة والمهمة ظهرت!");
    }
}