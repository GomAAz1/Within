using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MessageSystem : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI quoteText;
    public Image fadePanel;

    [Header("Targets")]
    public GameObject botsGroup; // اسحب الفولدر اللي فيه البوتات هنا

    private bool isPlayerNearby = false;
    private int pPressCount = 0;
    private bool isTransitioning = false;
    public static bool hasReadMessage = false; // فاريابل عالمي عشان الكرستالة تشوفه

    void Start()
    {
        if (mainText) mainText.text = "";
        if (quoteText) quoteText.text = "";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("ShadowPlayer"))
        {
            isPlayerNearby = true;
            if (!hasReadMessage) mainText.text = "Press P to Interact";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isPlayerNearby = false;
        if (!isTransitioning) { mainText.text = ""; quoteText.text = ""; }
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.P) && !isTransitioning)
        {
            if (pPressCount == 0) StartCoroutine(FirstPPress());
            else if (pPressCount == 1) StartCoroutine(SecondPPress());
        }
    }

    IEnumerator FirstPPress()
    {
        isTransitioning = true;
        pPressCount = 1;
        mainText.text = "";

        // 1. تسويد الشاشة بالبطئ
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 0.5f; // سرعة التسويد
            fadePanel.color = new Color(0, 0, 0, t);
            yield return null;
        }

        // 2. إظهار الرسائل فوق السواد
        mainText.text = "Press P again. Go to the bridge and see the change.";
        yield return new WaitForSeconds(0.5f);
        quoteText.text = "\"Do not compare yourself with others\"";

        isTransitioning = false;
    }

    IEnumerator SecondPPress()
    {
        isTransitioning = true;
        pPressCount = 2;
        mainText.text = "";
        quoteText.text = "";

        // 3. مسح البوتات
        if (botsGroup) botsGroup.SetActive(false);
        hasReadMessage = true;

        // 4. تفتيح الشاشة تاني
        float t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime * 0.5f;
            fadePanel.color = new Color(0, 0, 0, t);
            yield return null;
        }

        isTransitioning = false;
        Debug.Log("تم إنجاز المهمة، البوتات اختفت!");
    }
}