using UnityEngine;

public class LaserTrap : MonoBehaviour
{
    public Transform startPoint; // اسحب نقطة البداية هنا

    // الدالة دي بتشتغل أول ما حد يلمس الليزر
    private void OnTriggerEnter(Collider other)
    {
        // 1. لو اللي لمس الليزر هو "اللاعب الحقيقي"
        if (other.CompareTag("Player"))
        {
            Debug.Log("اللاعب الحقيقي لمس الليزر! عودة للبداية");
            ResetCharacter(other.gameObject);
        }

        // 2. لو اللي لمسه هو "الظل"
        if (other.CompareTag("ShadowPlayer"))
        {
            // هنا ممكن تقرر: هل الظل بيموت ولا بيعدي؟
            // لو عايزه "يعدي عادي"، سيب الحتة دي فاضية وماتكتبش حاجة.
            Debug.Log("الظل عدى بسلام من الليزر");
        }
    }

    void ResetCharacter(GameObject character)
    {
        CharacterController cc = character.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false; // قفل الفيزيا ثانية

        character.transform.position = startPoint.position;

        if (cc != null) cc.enabled = true; // رجع الفيزيا
    }
}