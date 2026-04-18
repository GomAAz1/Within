using UnityEngine;
using UnityEngine.SceneManagement;

public class CloseMenuScript : MonoBehaviour
{
    void Update()
    {
        // لو اللاعب داس Esc وهو جوه المنيو
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1f; // 1. رجع الوقت يشتغل تاني

            // 2. امسح "مشهد المنيو" بس وسيب اللعبة اللي تحتها زي ما هي
            SceneManager.UnloadSceneAsync("Demo1");

            // 3. اخفي الماوس عشان يرجع يلعب
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}