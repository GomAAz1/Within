using UnityEngine;

public class LaserSwitch : MonoBehaviour
{
    [Header("Settings")]
    public GameObject targetLaser; // اسحب الليزر اللي عايز تطفيه هنا
    public Color activeColor = Color.green;
    private Color originalColor;
    private MeshRenderer renderer;

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        if (renderer) originalColor = renderer.material.color;
    }

    // لما اللاعب يقف على الزرار
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("ShadowPlayer"))
        {
            targetLaser.SetActive(false); // الليزر المربوط بالزرار ده بس هو اللي يختفي
            if (renderer) renderer.material.color = activeColor;
            Debug.Log("تم تعطيل الليزر المربوط!");
        }
    }

    // لما اللاعب يسيب الزرار ويمشي (اختياري: لو عايز الليزر يرجع تاني)
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("ShadowPlayer"))
        {
            targetLaser.SetActive(true); // الليزر يرجع يشتغل لو ساب الزرار
            if (renderer) renderer.material.color = originalColor;
        }
    }
}