using UnityEngine;

public class LaserTrap : MonoBehaviour
{
    public Transform startPoint;

    private void OnTriggerEnter(Collider other)
    {
        // اللاعب الحقيقي بس هو اللي بيموت
        if (other.CompareTag("Player"))
        {
            Debug.Log("اللاعب الحقيقي مات! ريستارت للكل...");
            ResetEverything();
        }
    }

    void ResetEverything()
    {
        // 1. تشغيل كل الليزرات اللي اتقفلت
        LaserSwitch[] allSwitches = FindObjectsOfType<LaserSwitch>();
        foreach (LaserSwitch s in allSwitches)
        {
            if (s.targetLaser != null) s.targetLaser.SetActive(true);
        }

        // 2. تليبورت للاعبين
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