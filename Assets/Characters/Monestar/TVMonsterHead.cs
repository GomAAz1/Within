using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TVMonsterHead : MonoBehaviour
{
    [Header("1. Start Settings")]
    public float startDelay = 10f; // هينتظر 10 ثواني في الأول

    [Header("2. Patrol Settings (PingPong)")]
    public float horizontalRange = 60f;
    public float baseSpeed = 0.5f;
    public float lookDownAngle = 45f;
    public float transitionSpeed = 2f;

    [Header("3. Catch & Follow Sequence")]
    public float detectionAngle = 8.22f; // الزاوية اللي طلبتها
    public float detectionRange = 100f;
    public float trackingDuration = 5f;
    public float trackingSpeed = 10f;
    [Range(0, 360)] public float modelRotationOffset = 180f; // عشان لو بص وراه

    [Header("4. Visuals & UI")]
    public GameObject lightBeam;
    public Light spotLight;
    public Material lightBeamMaterial;
    public Color normalColor = Color.white;
    public Color alertColor = Color.red;
    public Image redFadeImage;
    public float fadeDuration = 0.5f;

    [Header("5. Sounds & References")]
    public AudioSource screamAudio;
    public AudioSource scanningSound;
    public Transform player, shadowPlayer, startPoint;

    private float currentSpeed, movementTimer, noiseOffset;
    private bool isScanning = false, isCaught = false, isInitialWaiting = true;
    private Color originalLightColor;

    void Start()
    {
        if (spotLight) originalLightColor = spotLight.color;
        noiseOffset = Random.Range(0f, 1000f);

        // تصفير الحالة في البداية
        isScanning = false; isCaught = false;
        SetLights(false);
        if (redFadeImage) { redFadeImage.color = new Color(1, 0, 0, 0); redFadeImage.gameObject.SetActive(false); }

        StartCoroutine(InitialStartRoutine());
    }

    IEnumerator InitialStartRoutine()
    {
        isInitialWaiting = true;
        yield return new WaitForSeconds(startDelay);

        SetLights(true);
        UpdateColors(normalColor);
        if (scanningSound) scanningSound.Play();

        isInitialWaiting = false;
        isScanning = true;
        movementTimer = 0;
    }

    void Update()
    {
        if (isInitialWaiting || isCaught || !isScanning) return;

        movementTimer += Time.deltaTime * baseSpeed;

        // حركة المسح الطبيعية (PingPong)
        float yaw = (Mathf.PingPong(movementTimer, 4f) - 2f) * (horizontalRange / 2f);

        // تطبيق الحركة محلياً لضمان عدم الشقلبة
        transform.localRotation = Quaternion.Euler(lookDownAngle, yaw, 0);

        CheckForPlayers();
    }

    void CheckForPlayers()
    {
        if (spotLight == null || isCaught) return;
        CheckTarget(player);
        CheckTarget(shadowPlayer);
    }

    void CheckTarget(Transform target)
    {
        if (target == null) return;
        float dist = Vector3.Distance(spotLight.transform.position, target.position);
        if (dist < detectionRange)
        {
            Vector3 dir = (target.position - spotLight.transform.position).normalized;
            if (Vector3.Angle(spotLight.transform.forward, dir) < detectionAngle / 2f)
            {
                StartCoroutine(TheKillerCatchSequence(target));
            }
        }
    }

    IEnumerator TheKillerCatchSequence(Transform target)
    {
        isCaught = true;
        isScanning = false;
        if (scanningSound) scanningSound.Stop();

        // 1. صرخة الرعب واللون الأحمر فوراً
        if (screamAudio) { screamAudio.volume = 1f; screamAudio.Play(); }
        UpdateColors(alertColor);

        // 2. المطاردة بالعين (5 ثواني) - مستحيل يهرب منك
        float t = 0;
        while (t < trackingDuration)
        {
            t += Time.deltaTime;
            Vector3 targetDir = target.position - transform.position;
            if (targetDir != Vector3.zero)
            {
                Quaternion lookRot = Quaternion.LookRotation(targetDir);
                lookRot *= Quaternion.Euler(0, modelRotationOffset, 0); // تصحيح الوش
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * trackingSpeed);
            }
            yield return null;
        }

        // 3. الفيد الأحمر
        if (redFadeImage) redFadeImage.gameObject.SetActive(true);
        float f = 0;
        while (f < 1) { f += Time.deltaTime * 2; redFadeImage.color = new Color(1, 0, 0, f); yield return null; }

        // 4. النقل وتصفير الماب
        ResetEverything();

        // 5. كسر اللوب: رجع الدماغ للنص فوراً وهي سودة
        transform.localRotation = Quaternion.Euler(lookDownAngle, 0, 0);

        yield return new WaitForSeconds(1f);

        // 6. تفتيح الشاشة والعودة للبداية (10 ثواني انتظار)
        UpdateColors(normalColor);
        while (f > 0) { f -= Time.deltaTime * 1.5f; redFadeImage.color = new Color(1, 0, 0, f); yield return null; }
        if (redFadeImage) redFadeImage.gameObject.SetActive(false);

        isCaught = false;
        StartCoroutine(InitialStartRoutine()); // يرجع ينتظر الـ 10 ثواني تانى
    }

    void ResetEverything()
    {
        if (player != null) Teleport(player);
        if (shadowPlayer != null) Teleport(shadowPlayer);

        LaserSwitch[] sw = FindObjectsOfType<LaserSwitch>(true);
        foreach (LaserSwitch s in sw) if (s.targetLaser != null) s.targetLaser.SetActive(true);
    }

    void Teleport(Transform t)
    {
        CharacterController cc = t.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        t.position = startPoint.position;
        if (cc != null) cc.enabled = true;
    }

    void SetLights(bool state)
    {
        if (lightBeam) lightBeam.SetActive(state);
        if (spotLight) spotLight.enabled = state;
    }

    void UpdateColors(Color c)
    {
        if (spotLight) spotLight.color = c;
        if (lightBeamMaterial)
        {
            lightBeamMaterial.SetColor("_BaseColor", c);
            lightBeamMaterial.SetColor("_EmissionColor", c * 15f);
        }
    }
}