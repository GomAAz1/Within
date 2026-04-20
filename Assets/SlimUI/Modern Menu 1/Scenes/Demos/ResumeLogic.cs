using UnityEngine;
using UnityEngine.SceneManagement;

public class ResumeLogic : MonoBehaviour
{
    public string pauseSceneName = "PauseMenu";

    public void BackToGame()
    {
        Time.timeScale = 1f;

        // 1. رجع كاميرا اللعبة والـ UI بتاعها
        GameObject mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCam != null) mainCam.SetActive(true);

        // بنور على سكريبت التريجر عشان يرجع الـ UI
        LevelPauseTrigger trigger = FindObjectOfType<LevelPauseTrigger>();
        if (trigger != null && trigger.gameUI != null) trigger.gameUI.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.UnloadSceneAsync(pauseSceneName);
    }

    // لو داس Esc تاني وهو في المنيو يرجع برضه
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) BackToGame();
    }
}