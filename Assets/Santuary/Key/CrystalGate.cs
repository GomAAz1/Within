using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CrystalGate : MonoBehaviour
{
    public GameObject key1, key2;
    public TextMeshProUGUI uiText;
    public Image fadeImage; // الـ FadePanel
    public string nextSceneName;

    private bool isPlayerNearby = false;
    private bool isTransitioning = false;

    void Start()
    {
        if (uiText) uiText.text = "";
        if (fadeImage) fadeImage.color = new Color(0, 0, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTransitioning) return; // لو بننقل خلاص مفيش داعي للكلام

        if (other.CompareTag("Player") || other.CompareTag("ShadowPlayer"))
        {
            isPlayerNearby = true;
            // السطر اللي كان ناقص ورجعناه دلوقت:
            if (uiText) uiText.text = "Press E to Interact";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("ShadowPlayer"))
        {
            isPlayerNearby = false;
            if (uiText && !isTransitioning) uiText.text = "";
        }
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !isTransitioning)
        {
            if (key1.activeSelf && key2.activeSelf)
            {
                StartCoroutine(FadeAndExit());
            }
            else
            {
                if (uiText) uiText.text = "Search for keys!";
            }
        }
    }

    IEnumerator FadeAndExit()
    {
        isTransitioning = true;
        if (uiText) uiText.text = ""; // اخفي النص عشان ميبانش فوق السواد

        float duration = 1.5f;
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(nextSceneName);
    }
}