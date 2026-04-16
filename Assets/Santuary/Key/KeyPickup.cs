using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public GameObject crystalKey; // اسحب المفتاح اللي فوق الكرستالة هنا
    public string targetTag;      // اكتب Player أو ShadowPlayer حسب المفتاح

    private void OnTriggerEnter(Collider other)
    {
        // لو الشخصية الصح هي اللي لمست المفتاح
        if (other.CompareTag(targetTag))
        {
            gameObject.SetActive(false); // اخفي المفتاح اللي في الماب
            if (crystalKey != null) crystalKey.SetActive(true); // اظهر المفتاح فوق الكرستالة
            Debug.Log(targetTag + " أخذ المفتاح!");
        }
    }
}