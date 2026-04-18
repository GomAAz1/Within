using UnityEngine;

public class LaserTrap : MonoBehaviour
{
    public Transform startPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ResetEverything();
        }
    }

    void ResetEverything()
    {
        // 1. ريست الليزرات + لون السويتش أصفر
        foreach (LaserSwitch s in FindObjectsOfType<LaserSwitch>(true))
        {
            if (s.targetLaser != null)
                s.targetLaser.SetActive(true);
            s.ResetToYellow();   // ★ يرجع أصفر
        }

        // 2. ريست المفاتيح
        foreach (KeyPickup kp in FindObjectsOfType<KeyPickup>(true))
            kp.ResetKey();       // ★ يرجع مفتاح الماب ويخفي مفتاح الكرستالة

        // 3. تليبورت اللاعبين
        Teleport(GameObject.FindWithTag("Player"));
        Teleport(GameObject.FindWithTag("ShadowPlayer"));
    }

    void Teleport(GameObject go)
    {
        if (go == null) return;
        CharacterController cc = go.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        go.transform.position = startPoint.position;
        if (cc != null) cc.enabled = true;
    }
}