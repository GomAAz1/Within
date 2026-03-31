using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CrystalInteraction : MonoBehaviour
{
    [Header("UI Objects")]
    public GameObject InteractPrompt;   // اسحب InteractPrompt هنا
    public GameObject SearchMessage;    // اسحب SearchMessage هنا
    public CanvasGroup DeepMessagePanel; // اسحب DeepMessagePanel هنا (تأكد من إضافة مكون Canvas Group له)

    [Header("Settings")]
    public string nextSceneName;        // اكتب اسم السين القادم هنا

    private bool isPlayerNearby = false;
    private bool firstInteractionDone = false;
    public static bool canFinishStage = false; // سيتغير من السكربت الثاني

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (!firstInteractionDone)
            {
                // المرة الأولى: تظهر رسالة البحث
                StartCoroutine(ShowSearchSequence());
                firstInteractionDone = true;
            }
            else if (canFinishStage)
            {
                // المرة الثانية (بعد مهمة حرف P): الانتقال للمرحلة القادمة بانميشن
                StartCoroutine(TransitionToNextScene());
            }
        }
    }

    IEnumerator ShowSearchSequence()
    {
        SearchMessage.SetActive(true);
        yield return new WaitForSeconds(3f); // تظهر لـ 3 ثواني
        SearchMessage.SetActive(false);
    }

    IEnumerator TransitionToNextScene()
    {
        DeepMessagePanel.gameObject.SetActive(true);
        // إخفاء النصوص الداخلية لعمل تأثير سواد فقط عند الانتقال
        DeepMessagePanel.transform.Find("text1").gameObject.SetActive(false);
        DeepMessagePanel.transform.Find("text2").gameObject.SetActive(false);

        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime;
            DeepMessagePanel.alpha = timer; // تلاشي للسواد
            yield return null;
        }
        SceneManager.LoadScene(nextSceneName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            InteractPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            InteractPrompt.SetActive(false);
        }
    }
}