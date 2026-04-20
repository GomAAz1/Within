using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class SleepSystem : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI objectiveText; // اسحب نص الكانفاس هنا
    public Image fadePanel;              // اسحب الصورة السوداء (Panel) هنا

    [Header("Settings")]
    public string nextSceneName;         // اسم المرحلة الجاية
    public float fadeSpeed = 0.5f;       // سرعة تسويد الشاشة

    private bool canSleep = false;
    private bool isDone = false;

    void Start()
    {
        // التأكد إن الشاشة شفافة في البداية
        if (fadePanel != null) fadePanel.color = new Color(0, 0, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // أول ما اللاعب يقرب من السرير
        if (other.CompareTag("Player") && !isDone)
        {
            canSleep = true;
            // السحر هنا: بنبدل النص القديم بالجديد
            objectiveText.text = "Press E to sleep";
            objectiveText.color = Color.yellow; // اختياري: نغير اللون عشان يشد انتباهه
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // لو اللاعب بعد عن السرير من غير ما ينام
        if (other.CompareTag("Player") && !isDone)
        {
            canSleep = false;
            // نرجع المهمة الأصلية
            objectiveText.text = "Objective: Go to sleep";
            objectiveText.color = Color.white;
        }
    }

    void Update()
    {
        // لو اللاعب في منطقة السرير وداس E
        if (canSleep && Input.GetKeyDown(KeyCode.E) && !isDone)
        {
            StartCoroutine(FadeToNextLevel());
        }
    }

    IEnumerator FadeToNextLevel()
    {
        isDone = true;
        canSleep = false;
        objectiveText.text = ""; // نمسح الكلام خالص وقت النوم

        float alpha = 0;

        // 1. تسويد الشاشة ببطء (Fade In)
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // 2. لحظة صمت قصيرة في السواد
        yield return new WaitForSeconds(1f);

        // 3. الانتقال للمرحلة الحقيقية
        SceneManager.LoadScene(nextSceneName);
    }
}