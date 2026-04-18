using UnityEngine;

public class LaserSwitch : MonoBehaviour
{
    [Header("الروابط")]
    public GameObject targetLaser;

    [Header("Colors (HDR)")]
    public Color activeGlowColor = Color.green;
    public Color originalGlowColor = Color.yellow;

    private MeshRenderer switchRenderer;
    private AudioSource switchAudio;
    private Material switchMat;

    void Start()
    {
        switchRenderer = GetComponent<MeshRenderer>();
        switchAudio = GetComponent<AudioSource>();

        if (switchRenderer != null)
        {
            switchMat = switchRenderer.material;
            switchMat.EnableKeyword("_EMISSION");
            ApplyColorAndGlow(originalGlowColor);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ShadowPlayer"))
        {
            if (targetLaser != null)
            {
                bool isNowActive = !targetLaser.activeSelf;
                targetLaser.SetActive(isNowActive);

                if (switchAudio != null) switchAudio.Play();

                Color targetColor = isNowActive ? originalGlowColor : activeGlowColor;
                ApplyColorAndGlow(targetColor);
            }
        }
    }

    // ★★★ الدالة الجديدة - الوحش بيستدعيها عند الريست
    public void ResetToYellow()
    {
        ApplyColorAndGlow(originalGlowColor);
    }

    void ApplyColorAndGlow(Color clr)
    {
        if (switchMat != null)
        {
            switchMat.SetColor("_BaseColor", clr);
            switchMat.SetColor("_EmissionColor", clr * 2f);
        }
    }
}