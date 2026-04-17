using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Level2Gate : MonoBehaviour
{
    public TextMeshProUGUI uiText;
    public Image fadePanel;
    public string nextSceneName;

    private bool isPlayerNearby = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("ShadowPlayer"))
        {
            isPlayerNearby = true;
            uiText.text = "Press E to Interact";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isPlayerNearby = false;
        uiText.text = "";
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            // بنشوف هل خلص مهمة الصورة ولا لسه
            if (MessageSystem.hasReadMessage)
            {
                StartCoroutine(FinalTransition());
            }
            else
            {
                uiText.text = "Goal not finished. Search for the message in the city across the bridge!";
            }
        }
    }

    IEnumerator FinalTransition()
    {
        uiText.text = "";
        float t = 0;
        // Fade Out (تسويد)
        while (t < 1)
        {
            t += Time.deltaTime;
            fadePanel.color = new Color(0, 0, 0, t);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nextSceneName);
    }
}