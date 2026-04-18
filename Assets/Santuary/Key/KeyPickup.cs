using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public GameObject crystalKey; // المفتاح اللي فوق الكرستالة
    public string targetTag;      // Player أو ShadowPlayer

    private bool isPicked = false; // ★ حالة المفتاح

    private void OnTriggerEnter(Collider other)
    {
        if (isPicked) return; // لو اتاخد خلاص مفيش داعي
        if (other.CompareTag(targetTag))
        {
            isPicked = true;
            gameObject.SetActive(false);                      // خفي مفتاح الماب
            if (crystalKey != null) crystalKey.SetActive(true); // ظهّر مفتاح الكرستالة
        }
    }

    // ★★★ الوحش بيستدعي الدالة دي عند الريست
    public void ResetKey()
    {
        isPicked = false;
        gameObject.SetActive(true);                            // رجّع مفتاح الماب
        if (crystalKey != null) crystalKey.SetActive(false);  // خفي مفتاح الكرستالة
    }
}