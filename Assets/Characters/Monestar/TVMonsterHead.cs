using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TVMonsterHead : MonoBehaviour
{
    [Header("Start Settings")]
    public float startDelay = 10f;

    [Header("Patrol Settings")]
    public float horizontalRange = 60f;
    public float baseSpeed = 0.5f;
    public float pauseDuration = 5f;
    public float lookDownAngle = 35f;
    public float transitionSpeed = 2f;
    [Tooltip("عدد اللفات قبل ما يفصل ويصحي تاني - غيّره براحتك")]
    public int maxScanCycles = 3;

    [Header("Randomness & Center Bias")]
    public bool useRandomMovement = true;
    public float randomnessSpeed = 0.8f;
    public float yawNoiseAmount = 25f;
    public float pitchNoiseAmount = 15f;
    [Range(0, 1)] public float centerBias = 0.4f;

    [Header("Detection Settings")]
    [Tooltip("زاوية الكشف - كبّرها عشان يمسك أسهل")]
    public float detectionAngle = 45f;
    public float detectionRange = 30f;
    public float trackingDuration = 5f;
    public float trackingSpeed = 6f;
    public float maxTrackingSpeed = 18f;
    public float trackingAcceleration = 2.5f;

    [Header("References")]
    public GameObject lightBeam;
    public Light spotLight;
    public Transform player;
    public Transform shadowPlayer;
    public Transform startPoint;

    [Header("Head Renderer - لون الراس")]
    public Renderer headRenderer;
    public int headMaterialIndex = 0;
    public Color headNormalColor = Color.white;
    public Color headAlertColor = Color.red;

    [Header("Beam Colors")]
    public Color normalBeamColor = Color.white;
    public Color alertBeamColor = Color.red;
    public Material lightBeamMaterial;

    [Header("Beam Scale")]
    public float beamScaleX = 1f;
    public float beamScaleY = 5f;
    public float beamScaleZ = 1f;

    [Header("Beam Position - تحريك البيم يدوي")]
    [Tooltip("X = يمين/شمال  |  Y = فوق/تحت  |  Z = قدام/ورا")]
    public float beamOffsetX = 0f;
    public float beamOffsetY = 0f;
    public float beamOffsetZ = 0f;

    [Header("Wake Up Light Delay")]
    [Tooltip("ثواني الانتظار بعد صوت الصحيان قبل ما يشغل الليزر والضوء")]
    public float lightActivationDelay = 2f;

    [Header("Red Fade UI")]
    public Image redFadeImage;
    public float fadeDuration = 0.5f;

    [Header("Monster Sounds")]
    public AudioSource wakeUpSound;
    public AudioSource scanningSound;
    public AudioSource detectionSound;

    // ── PRIVATE ──────────────────────────────────────────
    private float currentSpeed;
    private int scanCycles;
    private bool isScanning;
    private bool isInitialWaiting;
    private bool isTracking;
    private float movementTimer;
    private float noiseOffset;
    private Transform trackedTarget;
    private float currentTrackingSpeed;
    private Vector3 beamOriginalLocalPosition;
    private Material headMatInstance;
    private bool isLightOn = false; // ★ الضوء شغال ولا لا

    void Start()
    {
        currentSpeed = baseSpeed;
        noiseOffset = Random.Range(0f, 1000f);

        if (lightBeam != null)
            beamOriginalLocalPosition = lightBeam.transform.localPosition;

        if (headRenderer != null)
        {
            Material[] mats = headRenderer.materials;
            mats[headMaterialIndex] = new Material(mats[headMaterialIndex]);
            headRenderer.materials = mats;
            headMatInstance = headRenderer.materials[headMaterialIndex];
            SetHeadColor(headNormalColor);
        }

        if (redFadeImage != null)
        {
            redFadeImage.color = new Color(1f, 0f, 0f, 0f);
            redFadeImage.gameObject.SetActive(false);
        }

        ApplyBeamScale();
        SetBeamColor(normalBeamColor);
        SetSpotColor(normalBeamColor);
        transform.localRotation = Quaternion.Euler(lookDownAngle, 0f, 0f);
        StartCoroutine(InitialStartRoutine());
    }

    void ApplyBeamScale()
    {
        if (lightBeam == null) return;
        lightBeam.transform.localScale = new Vector3(beamScaleX, beamScaleY, beamScaleZ);
        // ★ بنضيف الـ offset على الـ position الأصلي بالظبط
        lightBeam.transform.localPosition = beamOriginalLocalPosition + new Vector3(beamOffsetX, beamOffsetY, beamOffsetZ);
    }

    // ── COROUTINES ────────────────────────────────────────
    IEnumerator InitialStartRoutine()
    {
        isInitialWaiting = true;
        isScanning = false;
        isTracking = false;
        SetBeamColor(normalBeamColor);
        SetSpotColor(normalBeamColor);
        SetHeadColor(headNormalColor);
        SetLight(false);

        yield return new WaitForSeconds(startDelay);

        if (wakeUpSound != null) wakeUpSound.Play();
        yield return StartCoroutine(MoveHeadToPosition(lookDownAngle, 0f));

        // ★ يتحرك بدون شعاع وبدون كشف للمدة المحددة
        isInitialWaiting = false;
        isScanning = true;
        isLightOn = false;
        movementTimer = 0f;
        scanCycles = 0;

        yield return new WaitForSeconds(lightActivationDelay);

        // ★ بعد المدة يشغل الشعاع والصوت ويبدأ الكشف
        isLightOn = true;
        SetLight(true);
        SetBeamColor(normalBeamColor);
        SetSpotColor(normalBeamColor);
        if (scanningSound != null) scanningSound.Play();
    }

    IEnumerator TransitionToSleep()
    {
        isScanning = false;
        if (scanningSound != null) scanningSound.Stop();

        yield return StartCoroutine(MoveHeadToPosition(lookDownAngle, 0f));
        SetLight(false);
        isLightOn = false;
        SetBeamColor(normalBeamColor);
        SetSpotColor(normalBeamColor);
        SetHeadColor(headNormalColor);

        yield return new WaitForSeconds(pauseDuration);
        currentSpeed = Mathf.Min(currentSpeed + 0.1f, 1.5f);

        if (wakeUpSound != null) wakeUpSound.Play();
        yield return StartCoroutine(MoveHeadToPosition(lookDownAngle, 0f));

        isLightOn = true;
        SetLight(true);
        SetBeamColor(normalBeamColor);
        SetSpotColor(normalBeamColor);
        if (scanningSound != null) scanningSound.Play();

        scanCycles = 0;
        movementTimer = 0f;
        isScanning = true;
    }

    IEnumerator TrackingRoutine(Transform target)
    {
        isScanning = false;
        isTracking = true;
        trackedTarget = target;

        isLightOn = true;
        SetLight(true);
        SetSpotColor(alertBeamColor);
        SetBeamColor(alertBeamColor);
        SetHeadColor(headAlertColor);
        if (scanningSound != null) scanningSound.Stop();
        if (detectionSound != null) detectionSound.Play();

        currentTrackingSpeed = trackingSpeed;

        float elapsed = 0f;
        while (elapsed < trackingDuration)
        {
            elapsed += Time.deltaTime;
            currentTrackingSpeed = Mathf.Min(
                currentTrackingSpeed + trackingAcceleration * Time.deltaTime,
                maxTrackingSpeed);

            if (trackedTarget != null)
                TrackTarget(trackedTarget.position);

            yield return null;
        }

        SetSpotColor(normalBeamColor);
        SetBeamColor(normalBeamColor);
        SetHeadColor(headNormalColor);

        yield return StartCoroutine(DoRedFade());
        FullReset();
    }

    IEnumerator DoRedFade()
    {
        if (redFadeImage == null) yield break;
        redFadeImage.gameObject.SetActive(true);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            redFadeImage.color = new Color(1f, 0f, 0f, Mathf.Clamp01(t / fadeDuration));
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            redFadeImage.color = new Color(1f, 0f, 0f, Mathf.Clamp01(1f - t / fadeDuration));
            yield return null;
        }
        redFadeImage.gameObject.SetActive(false);
    }

    IEnumerator MoveHeadToPosition(float targetPitch, float targetYaw)
    {
        Quaternion targetRot = Quaternion.Euler(targetPitch, targetYaw, 0f);
        while (Quaternion.Angle(transform.localRotation, targetRot) > 0.1f)
        {
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation, targetRot, Time.deltaTime * transitionSpeed);
            yield return null;
        }
        transform.localRotation = targetRot;
    }

    // ── UPDATE ────────────────────────────────────────────
    void Update()
    {
        if (isInitialWaiting || isTracking || !isScanning) return;

        movementTimer += Time.deltaTime * currentSpeed;

        float rawYaw = (Mathf.PingPong(movementTimer, 4f) - 2f) * (horizontalRange / 2f);
        float yaw = Mathf.Lerp(rawYaw, 0f, centerBias);
        float pitch = lookDownAngle;

        if (useRandomMovement)
        {
            float nx = (Mathf.PerlinNoise(Time.time * randomnessSpeed + noiseOffset, 0f) - 0.5f) * 2f;
            float ny = (Mathf.PerlinNoise(0f, Time.time * randomnessSpeed + noiseOffset) - 0.5f) * 2f;
            yaw += nx * yawNoiseAmount;
            pitch += ny * pitchNoiseAmount;
        }

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            Quaternion.Euler(pitch, yaw, 0f),
            Time.deltaTime * 5f);

        if (movementTimer >= 8f)
        {
            movementTimer = 0f;
            scanCycles++;
            if (scanCycles >= maxScanCycles)
                StartCoroutine(TransitionToSleep());
        }

        CheckForPlayers();
    }

    // ── DETECTION ★★★ بيمسك أي لاعب في المنطقة ─────────
    void CheckForPlayers()
    {
        if (spotLight == null || isTracking || !isLightOn) return; // ★ مش يقفش لو الضوء مطفي

        // ★ بيجرب Player الأول، لو مش موجود بيجرب ShadowPlayer
        Transform hit = GetDetectedTarget(player) ?? GetDetectedTarget(shadowPlayer);
        if (hit != null)
            StartCoroutine(TrackingRoutine(hit));
    }

    Transform GetDetectedTarget(Transform target)
    {
        if (target == null || !target.gameObject.activeInHierarchy) return null;
        if (spotLight == null) return null;

        Transform ld = spotLight.transform;
        float dist = Vector3.Distance(ld.position, target.position);
        if (dist >= detectionRange) return null;

        Vector3 dir = (target.position - ld.position).normalized;
        if (Vector3.Angle(ld.forward, dir) >= detectionAngle) return null;

        // ★ لو الـ Raycast مامسكش حاجه = مفيش حاجة حاجبة = يمسكه
        RaycastHit rh;
        if (!Physics.Raycast(ld.position + ld.forward * 0.5f, dir, out rh, dist))
            return target; // مفيش حاجة حاجبة، يمسكه

        // لو في حاجة حاجبة، شوف هي اللاعب نفسه
        if (rh.collider.CompareTag("Player") || rh.collider.CompareTag("ShadowPlayer"))
            return target;

        return null;
    }

    // ★★★ TrackTarget
    void TrackTarget(Vector3 worldTargetPos)
    {
        if (spotLight == null) return;

        Vector3 toTarget = worldTargetPos - spotLight.transform.position;
        if (toTarget.sqrMagnitude < 0.001f) return;

        Quaternion deltaRot = Quaternion.FromToRotation(
            spotLight.transform.forward, toTarget.normalized);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            deltaRot * transform.rotation,
            Time.deltaTime * currentTrackingSpeed);
    }

    // ── FULL RESET ────────────────────────────────────────
    void FullReset()
    {
        StopAllCoroutines();

        isTracking = false;
        isScanning = false;
        isInitialWaiting = false;
        trackedTarget = null;
        currentTrackingSpeed = trackingSpeed;

        ApplyBeamScale();
        SetBeamColor(normalBeamColor);
        SetSpotColor(normalBeamColor);
        SetHeadColor(headNormalColor);

        if (player != null) TeleportTo(player, startPoint.position);
        if (shadowPlayer != null) TeleportTo(shadowPlayer, startPoint.position);

        // ★ ريست الليزر + لون السويتش أصفر
        foreach (LaserSwitch s in FindObjectsOfType<LaserSwitch>(true))
        {
            if (s.targetLaser != null)
                s.targetLaser.SetActive(true);
            s.ResetToYellow();
        }

        // ★★★ ريست المفاتيح عن طريق KeyPickup مباشرة
        foreach (KeyPickup kp in FindObjectsOfType<KeyPickup>(true))
            kp.ResetKey();

        transform.localRotation = Quaternion.Euler(lookDownAngle, 0f, 0f);
        SetLight(false);
        currentSpeed = baseSpeed;
        scanCycles = 0;
        movementTimer = 0f;

        StartCoroutine(InitialStartRoutine());
    }

    // ── HELPERS ───────────────────────────────────────────
    void SetLight(bool active)
    {
        if (lightBeam) lightBeam.SetActive(active);
        if (spotLight) spotLight.enabled = active;
    }

    void SetSpotColor(Color c)
    {
        if (spotLight) spotLight.color = c;
    }

    void SetBeamColor(Color c)
    {
        if (lightBeamMaterial == null) return;
        lightBeamMaterial.color = c;
        if (lightBeamMaterial.HasProperty("_EmissionColor"))
            lightBeamMaterial.SetColor("_EmissionColor", c);
    }

    void SetHeadColor(Color c)
    {
        if (headMatInstance == null) return;
        headMatInstance.color = c;
        if (headMatInstance.HasProperty("_EmissionColor"))
            headMatInstance.SetColor("_EmissionColor", c * 0.5f);
    }

    void TeleportTo(Transform t, Vector3 pos)
    {
        CharacterController cc = t.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;
        t.position = pos;
        if (cc) cc.enabled = true;
    }
}