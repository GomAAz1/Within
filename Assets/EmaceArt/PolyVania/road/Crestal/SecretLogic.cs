using UnityEngine;
using System.Collections.Generic;

public class SecretLogic : MonoBehaviour
{
    public GameObject DeepMessagePanel; // اسحب DeepMessagePanel هنا
    public List<GameObject> bots;       // اسحب Player1 إلى Player6 هنا

    private int pPressCount = 0;
    private bool isPlayerAtMessage = false;

    void Update()
    {
        if (isPlayerAtMessage && Input.GetKeyDown(KeyCode.P))
        {
            pPressCount++;

            if (pPressCount == 1)
            {
                // الضغطة الأولى: تفعيل السواد والنصوص (text1, text2)
                DeepMessagePanel.SetActive(true);
                // تأكد أن الشفافية (Alpha) هي 1 لرؤية السواد والرسائل
                if (DeepMessagePanel.GetComponent<CanvasGroup>())
                    DeepMessagePanel.GetComponent<CanvasGroup>().alpha = 1;
            }
            else if (pPressCount == 2)
            {
                // الضغطة الثانية: إخفاء السواد، مسح البوتات، وتفعيل إمكانية إنهاء المرحلة
                DeepMessagePanel.SetActive(false);
                DeleteBots();
                CrystalInteraction.canFinishStage = true; // السماح للكرستالة بإنهاء السين
            }
        }
    }

    void DeleteBots()
    {
        foreach (GameObject bot in bots)
        {
            if (bot != null) bot.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerAtMessage = true;
    }
}