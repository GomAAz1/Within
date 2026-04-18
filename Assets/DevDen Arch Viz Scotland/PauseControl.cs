using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManagerScript : MonoBehaviour
{
    [Header("UI Setup")]
    public GameObject pauseMenuPanel; // اسحب لوحة المنيو هنا

    [Header("Settings")]
    public string mainMenuSceneName = "Demo1"; // اكتب اسم سين المنيو الرئيسي هنا

    private bool isPaused = false;

    void Start()
    {
        // التأكد أن المنيو مخفية عند بداية اللعبة
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

    void Update()
    {
        // عند الضغط على زر ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true); // إظهار المنيو
        Time.timeScale = 0f;           // إيقاف زمن اللعبة تماماً
        isPaused = true;

        // إظهار الماوس وتحريره من قفل الكاميرا
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false); // إخفاء المنيو
        Time.timeScale = 1f;            // تشغيل زمن اللعبة مرة أخرى
        isPaused = false;

        // إخفاء الماوس وقفل الكاميرا للعودة للعب
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; // مهم جداً: إرجاع الوقت للطبيعي قبل الانتقال
        SceneManager.LoadScene(mainMenuSceneName);
    }
}