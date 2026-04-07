using UnityEngine;
using Unity.Cinemachine;

public class ShadowCameraManager : MonoBehaviour
{
    [Header("References")]
    public ShadowPlayer shadowPlayerScript;
    public CinemachineCamera camReal;
    public CinemachineCamera camShadow;

    [Header("Mouse Settings")]
    public float sensitivityX = 2f;
    public float sensitivityY = 2f;
    [Range(0, 1)] public float rotationSmoothTime = 0.15f;

    [Header("Rotation Limits")]
    public float panLimit = 60f;
    public float minY = -20f;
    public float maxY = 40f;

    private float currentPanOffset = 0f;
    private float currentTilt = 0f;
    private bool lastShadowActive;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentTilt = camReal.transform.eulerAngles.x;
        lastShadowActive = shadowPlayerScript.IsShadowActive();
        SyncPriority();
    }

    void Update()
    {
        if (shadowPlayerScript == null) return;

        // 1. التبديل الذكي بالأولوية (يمنع الغطس تماماً)
        if (shadowPlayerScript.IsShadowActive() != lastShadowActive)
        {
            lastShadowActive = shadowPlayerScript.IsShadowActive();
            currentPanOffset = 0; // تصفير الإزاحة عند التبديل لمنع اللخبطة
            SyncPriority();
        }

        // 2. قراءة الماوس
        currentPanOffset += Input.GetAxis("Mouse X") * sensitivityX;
        currentPanOffset = Mathf.Clamp(currentPanOffset, -panLimit, panLimit);

        currentTilt -= Input.GetAxis("Mouse Y") * sensitivityY;
        currentTilt = Mathf.Clamp(currentTilt, minY, maxY);

        // 3. تطبيق الحركة للسينماشين بناءً على القائد الحالي
        CharacterController leader = lastShadowActive ? shadowPlayerScript.shadowCC : shadowPlayerScript.realCC;
        CinemachineCamera activeCam = lastShadowActive ? camShadow : camReal;

        var panTilt = activeCam.GetComponent<CinemachinePanTilt>();
        if (panTilt != null)
        {
            panTilt.PanAxis.Value = leader.transform.eulerAngles.y + currentPanOffset;
            panTilt.TiltAxis.Value = currentTilt;
        }

        // 4. تدوير اللاعب بنعومة لوش الكاميرا (عشان الـ WASD يشتغل صح)
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
        {
            float targetAngle = leader.transform.eulerAngles.y + currentPanOffset;
            leader.transform.rotation = Quaternion.Slerp(leader.transform.rotation, Quaternion.Euler(0, targetAngle, 0), rotationSmoothTime);
            currentPanOffset = Mathf.Lerp(currentPanOffset, 0, rotationSmoothTime);
        }
    }

    void SyncPriority()
    {
        // الطريقة الصح للتبديل: الكاميرا اللي priority بتاعها 10 هي اللي بتظهر
        camReal.Priority = lastShadowActive ? 0 : 10;
        camShadow.Priority = lastShadowActive ? 10 : 0;
    }
}