using UnityEngine;
using System.Collections;

public class TVMonsterHead : MonoBehaviour
{
    [Header("Start Settings")]
    public float startDelay = 10f;

    [Header("Patrol Settings")]
    public float horizontalRange = 60f;
    public float baseSpeed = 0.5f;
    public float pauseDuration = 5f;
    public float lookDownAngle = 50f;
    public float transitionSpeed = 2f;

    [Header("Randomness & Center Bias")]
    public bool useRandomMovement = true;
    public float randomnessSpeed = 0.8f;
    public float yawNoiseAmount = 25f;
    public float pitchNoiseAmount = 15f;
    [Range(0, 1)] public float centerBias = 0.4f;

    [Header("References")]
    public GameObject lightBeam;
    public Light spotLight;
    public Transform player;
    public Transform shadowPlayer;
    public Transform startPoint;

    private float currentSpeed;
    private int scanCycles = 0;
    private bool isScanning = false;
    private bool isInitialWaiting = true;
    private float movementTimer = 0f;
    private float noiseOffset;

    void Start()
    {
        currentSpeed = baseSpeed;
        noiseOffset = Random.Range(0, 1000);
        transform.localRotation = Quaternion.Euler(lookDownAngle, 0, 0);
        StartCoroutine(InitialStartRoutine());
    }

    IEnumerator InitialStartRoutine()
    {
        isInitialWaiting = true;
        if (lightBeam) lightBeam.SetActive(false);
        if (spotLight) spotLight.enabled = false;
        yield return new WaitForSeconds(startDelay);
        yield return StartCoroutine(MoveHeadToPosition(lookDownAngle, 0));
        if (lightBeam) lightBeam.SetActive(true);
        if (spotLight) spotLight.enabled = true;
        isInitialWaiting = false;
        isScanning = true;
    }

    void Update()
    {
        if (isInitialWaiting || !isScanning) return;

        movementTimer += Time.deltaTime * currentSpeed;

        float rawYaw = (Mathf.PingPong(movementTimer, 4f) - 2f) * (horizontalRange / 2f);
        float yaw = Mathf.Lerp(rawYaw, 0, centerBias);
        float pitch = lookDownAngle;

        if (useRandomMovement)
        {
            float noiseX = (Mathf.PerlinNoise(Time.time * randomnessSpeed + noiseOffset, 0) - 0.5f) * 2f;
            float noiseY = (Mathf.PerlinNoise(0, Time.time * randomnessSpeed + noiseOffset) - 0.5f) * 2f;
            yaw += noiseX * yawNoiseAmount;
            pitch += noiseY * pitchNoiseAmount;
        }

        Quaternion targetRot = Quaternion.Euler(pitch, yaw, 0);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * 5f);

        if (movementTimer >= 8f)
        {
            movementTimer = 0;
            scanCycles++;
            if (scanCycles >= 3) StartCoroutine(TransitionToSleep());
        }

        CheckForPlayers();
    }

    IEnumerator TransitionToSleep()
    {
        isScanning = false;
        yield return StartCoroutine(MoveHeadToPosition(lookDownAngle, 0));
        if (lightBeam) lightBeam.SetActive(false);
        if (spotLight) spotLight.enabled = false;
        yield return new WaitForSeconds(pauseDuration);
        currentSpeed += 0.1f;
        if (currentSpeed > 1.5f) currentSpeed = baseSpeed;
        yield return StartCoroutine(MoveHeadToPosition(lookDownAngle, 0));
        if (lightBeam) lightBeam.SetActive(true);
        if (spotLight) spotLight.enabled = true;
        scanCycles = 0;
        movementTimer = 0;
        isScanning = true;
    }

    IEnumerator MoveHeadToPosition(float targetPitch, float targetYaw)
    {
        Quaternion targetRot = Quaternion.Euler(targetPitch, targetYaw, 0);
        while (Quaternion.Angle(transform.localRotation, targetRot) > 0.1f)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * transitionSpeed);
            yield return null;
        }
        transform.localRotation = targetRot;
    }

    void CheckForPlayers()
    {
        if (spotLight == null) return;
        CheckTarget(player, spotLight.transform);
        CheckTarget(shadowPlayer, spotLight.transform);
    }

    void CheckTarget(Transform target, Transform lightDir)
    {
        if (target == null) return;

        float distance = Vector3.Distance(lightDir.position, target.position);

        // لو اللاعب في نطاق طول الكشاف
        if (distance < spotLight.range)
        {
            Vector3 directionToTarget = (target.position - lightDir.position).normalized;

            // حساب الزاوية بين وش الكشاف ومكان اللاعب
            float angle = Vector3.Angle(lightDir.forward, directionToTarget);

            // --- العودة للدقة الأصلية ---
            // بنقسم الـ 8.22 على 2 عشان نجيب زاوية الميل من النص
            float detectionAngle = spotLight.spotAngle / 2f;

            if (angle < detectionAngle)
            {
                RaycastHit hit;
                // بنبدأ الشعاع من قدام اللمبة بـ 2 متر عشان م يخبطش في الوحش نفسه
                Vector3 rayStart = lightDir.position + lightDir.forward * 2f;

                if (Physics.Raycast(rayStart, directionToTarget, out hit, distance))
                {
                    if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("ShadowPlayer"))
                    {
                        ResetPlayers();
                    }
                }
            }
        }
    }

    void ResetPlayers()
    {
        Debug.Log("تم رصد اللاعب! إعادة ضبط المرحلة...");
        if (player != null) Teleport(player);
        if (shadowPlayer != null) Teleport(shadowPlayer);

        LaserSwitch[] allSwitches = FindObjectsOfType<LaserSwitch>(true);
        foreach (LaserSwitch s in allSwitches)
        {
            if (s.targetLaser != null) s.targetLaser.SetActive(true);
        }
    }

    void Teleport(Transform target)
    {
        CharacterController cc = target.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        target.position = startPoint.position;
        if (cc != null) cc.enabled = true;
    }
}