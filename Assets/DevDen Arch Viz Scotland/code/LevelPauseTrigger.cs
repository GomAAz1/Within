using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelPauseTrigger : MonoBehaviour
{
    public string pauseSceneName = "Pause";
    public GameObject gameUI; // ★ اسحب الكانفاس بتاع كلمة go to sleep هنا ★
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused) PauseGame();
            else ResumeFromCode();
        }
    }

    void PauseGame()
    {
        isPaused = true;
        // 1. إخفاء واجهة اللعبة (عشان go to sleep تختفي والموس يشتغل)
        if (gameUI != null) gameUI.SetActive(false);

        // 2. تحميل المنيو
        SceneManager.LoadScene(pauseSceneName, LoadSceneMode.Additive);

        // 3. وقف الوقت وتحرير الماوس
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeFromCode()
    {
        isPaused = false;
        Time.timeScale = 1f;

        // 4. إظهار واجهة اللعبة تاني لما نرجع
        if (gameUI != null) gameUI.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.UnloadSceneAsync(pauseSceneName);
    }
}