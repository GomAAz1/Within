using UnityEngine;
using Unity.Cinemachine;

public class PlayerCameraManager : MonoBehaviour
{
    [Header("References")]
    public CharacterController playerCC; // اسحب اللاعب هنا
    public CinemachineCamera vcam;       // اسحب كاميرا السينماشين هنا

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

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentTilt = vcam.transform.eulerAngles.x;
    }

    void Update()
    {
        if (playerCC == null || vcam == null) return;

        // 1. قراءة الماوس
        currentPanOffset += Input.GetAxis("Mouse X") * sensitivityX;
        currentPanOffset = Mathf.Clamp(currentPanOffset, -panLimit, panLimit);

        currentTilt -= Input.GetAxis("Mouse Y") * sensitivityY;
        currentTilt = Mathf.Clamp(currentTilt, minY, maxY);

        // 2. تطبيق الحركة للسينماشين
        var panTilt = vcam.GetComponent<CinemachinePanTilt>();
        if (panTilt != null)
        {
            panTilt.PanAxis.Value = playerCC.transform.eulerAngles.y + currentPanOffset;
            panTilt.TiltAxis.Value = currentTilt;
        }

        // 3. تدوير اللاعب لوش الكاميرا عند الحركة
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
        {
            float targetAngle = playerCC.transform.eulerAngles.y + currentPanOffset;
            playerCC.transform.rotation = Quaternion.Slerp(playerCC.transform.rotation, Quaternion.Euler(0, targetAngle, 0), rotationSmoothTime);
            currentPanOffset = Mathf.Lerp(currentPanOffset, 0, rotationSmoothTime);
        }
    }
}