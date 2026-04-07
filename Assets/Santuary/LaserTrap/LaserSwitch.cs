using UnityEngine;

public class LaserSwitch : MonoBehaviour
{
    [Header("الروابط")]
    public GameObject targetLaser; // اسحب شريط الليزر الأحمر من الهيراركي هنا

    private void OnTriggerEnter(Collider other)
    {
        // بنختبر لو الضل هو اللي لمس "الزرار"
        if (other.CompareTag("ShadowPlayer"))
        {
            if (targetLaser != null)
            {
                targetLaser.SetActive(false); // إخفاء الليزر تماماً
                Debug.Log("!!! زرار الليزر اشتغل والضل طفاه دلوقت !!!");

                // تغيير لون الزرار للاخضر كدليل
                if (GetComponent<MeshRenderer>() != null)
                    GetComponent<MeshRenderer>().material.color = Color.green;
            }
            else
            {
                Debug.LogError("يا معلم أنت مش ساحب الليزر في خانة Target Laser في الزرار!");
            }
        }
    }
}