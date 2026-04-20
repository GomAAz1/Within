using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FinaleManager : MonoBehaviour
{
    [Header("References")]
    public PlayableDirector endDirector;
    public Image fadeImage;
    public string mainMenuSceneName;

    [Header("Timing")]
    public float startFadeTime = 2.5f;
    public float endFadeTime = 2.5f;

    private bool isTransitioningToMenu = false;

    void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.color = new Color(0, 0, 0, 1);
        }

        // تحضير الشخصيات خلف السواد لمنع الرعشة
        if (endDirector != null)
        {
            endDirector.time = 0.1f;
            endDirector.Evaluate();
        }

        StartCoroutine(BeginFinale());
    }

    IEnumerator BeginFinale()
    {
        yield return new WaitForSeconds(1f);

        if (endDirector != null) endDirector.Play();

        float timer = 0;
        while (timer < startFadeTime)
        {
            timer += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, timer / startFadeTime));
            yield return null;
        }
    }

    void OnEnable() { if (endDirector != null) endDirector.stopped += OnFinaleStopped; }
    void OnDisable() { if (endDirector != null) endDirector.stopped -= OnFinaleStopped; }

    void OnFinaleStopped(PlayableDirector aDirector)
    {
        if (!isTransitioningToMenu) StartCoroutine(ExitToMainMenu());
    }

    IEnumerator ExitToMainMenu()
    {
        isTransitioningToMenu = true;
        float timer = 0;

        while (timer < endFadeTime)
        {
            timer += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, timer / endFadeTime));
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(mainMenuSceneName);
    }
}