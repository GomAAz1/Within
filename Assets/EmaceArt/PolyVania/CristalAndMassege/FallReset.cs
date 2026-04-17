using UnityEngine;

public class FallReset : MonoBehaviour
{
    public Transform spawnPoint; // اسحب نقطة البداية هنا

    private void OnTriggerEnter(Collider other)
    {
        // بنشوف هل اللي وقع هو اللاعب الحقيقي أو الضل
        if (other.CompareTag("Player") || other.CompareTag("ShadowPlayer"))
        {
            Debug.Log(other.name + " وقع! إعادة للبداية.");
            Teleport(other.gameObject);
        }
    }

    void Teleport(GameObject go)
    {
        CharacterController cc = go.GetComponent<CharacterController>();

        // تكة مهمة: لازم نقفل الكنترولر ثانية عشان الفيزياء م تمنعش النقل
        if (cc != null) cc.enabled = false;

        go.transform.position = spawnPoint.position;
        go.transform.rotation = spawnPoint.rotation;

        if (cc != null) cc.enabled = true;
    }
}