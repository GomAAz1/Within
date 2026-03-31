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
    public float transitionSpeed = 2f; // سرعة "الصحية" و "النومة"

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

        // بنبدأ اللعبة بوضع الراحة
        transform.localRotation = Quaternion.Euler(lookDownAngle, 0, 0);
        StartCoroutine(InitialStartRoutine());
    }

    IEnumerator InitialStartRoutine()
    {
        isInitialWaiting = true;
        if (lightBeam) lightBeam.SetActive(false);
        if (spotLight) spotLight.enabled = false;

        yield return new WaitForSeconds(startDelay);

        // "الصحية" بنعومة
        yield return StartCoroutine(MoveHeadToPosition(lookDownAngle, 0));

        // شغل النور وابدأ المسح
        if (lightBeam) lightBeam.SetActive(true);
        if (spotLight) spotLight.enabled = true;

        isInitialWaiting = false;
        isScanning = true;
    }

    void Update()
    {
        if (isInitialWaiting || !isScanning) return;

        movementTimer += Time.deltaTime * currentSpeed;

        // الحركة العشوائية المطورة
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

        // تطبيق الحركة بسلاسة
        Quaternion targetRot = Quaternion.Euler(pitch, yaw, 0);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * 5f);

        // عداد المسحات
        if (movementTimer >= 8f)
        { // دورة كاملة
            movementTimer = 0;
            scanCycles++;
            if (scanCycles >= 3)
            {
                StartCoroutine(TransitionToSleep());
            }
        }

        CheckForPlayers();
    }

    // كوروتين العودة للنوم بنعومة
    IEnumerator TransitionToSleep()
    {
        isScanning = false;
        Debug.Log("الوحش بيبدأ ينام...");

        // العودة للسنتر بنعومة قبل إطفاء النور
        yield return StartCoroutine(MoveHeadToPosition(lookDownAngle, 0));

        // اطفي النور بعد ما يوصل للسنتر
        if (lightBeam) lightBeam.SetActive(false);
        if (spotLight) spotLight.enabled = false;

        yield return new WaitForSeconds(pauseDuration);

        // يرجع يصحى تاني
        currentSpeed += 0.1f;
        if (currentSpeed > 1.5f) currentSpeed = baseSpeed;

        // "الصحية" التانية
        yield return StartCoroutine(MoveHeadToPosition(lookDownAngle, 0));

        if (lightBeam) lightBeam.SetActive(true);
        if (spotLight) spotLight.enabled = true;

        scanCycles = 0;
        movementTimer = 0;
        isScanning = true;
    }

    // دالة مساعدة لتحريك الراس لأي زاوية بنعومة
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
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 200f))
        {
            if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("ShadowPlayer"))
            {
                ResetPlayers();
            }
        }
    }

    void ResetPlayers()
    {
        if (player != null) Teleport(player);
        if (shadowPlayer != null) Teleport(shadowPlayer);
    }

    void Teleport(Transform target)
    {
        CharacterController cc = target.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        target.position = startPoint.position;
        if (cc != null) cc.enabled = true;
    }
}