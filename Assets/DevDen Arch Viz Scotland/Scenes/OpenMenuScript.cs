using UnityEngine;
using UnityEngine.SceneManagement; // مهمة عشان نغير المشاهد

public class OpenMenuScript : MonoBehaviour
{
    void Update()
    {
        // لو اللاعب داس Esc وهو بيلعب
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0f; // 1. وقف الوقت (عشان اللعبة تثبت مكانها)

            // 2. حمل المنيو "فوق" اللعبة (Additive تعني إضافة مش استبدال)
            SceneManager.LoadScene("Demo1", LoadSceneMode.Additive);

            // 3. أظهر الماوس عشان يعرف يختار من المنيو
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}