using UnityEngine;
using UnityEngine.SceneManagement;

public class GameToMenu : MonoBehaviour
{
    public string menuSceneName = "Demo1"; // اسم سين المنيو بتاعك

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0f; // وقف اللعبة
            // تحميل المنيو فوق اللعبة (Additive)
            SceneManager.LoadScene(menuSceneName, LoadSceneMode.Additive);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}