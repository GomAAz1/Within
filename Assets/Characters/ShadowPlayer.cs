using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class ShadowPlayer : MonoBehaviour
{
    struct Command { public float h, v; public bool run, jump; public Quaternion rot; public float time; }

    [Header("Characters Setup")]
    public CharacterController realCC;
    public CharacterController shadowCC;
    public Volume shadowVolume;

    [Header("Footsteps Settings")]
    public AudioSource realAudio;
    public AudioSource shadowAudio;
    public AudioClip[] footstepClips;
    public float walkStepInterval = 0.5f; // سرعة الخطوات في المشي
    public float runStepInterval = 0.3f;  // سرعة الخطوات في الجري

    [Header("Visibility Layers")]
    public LayerMask realMask;
    public LayerMask shadowMask;

    [Header("Keys & Settings")]
    public KeyCode switchKey = KeyCode.Tab;
    public KeyCode freezeKey = KeyCode.F;
    public KeyCode recallKey = KeyCode.R;
    public float followDelay = 1.0f;
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 8f;
    public float gravity = 30f;

    private Queue<Command> history = new Queue<Command>();
    private Animator realAnim, shadowAnim;
    private bool isShadowActive = false, isFrozen = false, isRecalling = false;
    private float realYV = -2f, shadowYV = -2f;

    // عدادات الخطوات لكل لاعب
    private float realStepTimer, shadowStepTimer;

    void Start()
    {
        realAnim = realCC.GetComponent<Animator>();
        shadowAnim = shadowCC.GetComponent<Animator>();
        Physics.IgnoreCollision(realCC, shadowCC);
        Cursor.lockState = CursorLockMode.Locked;
        UpdateVisuals();
    }

    void Update()
    {
        HandleInput();

        CharacterController leader = isShadowActive ? shadowCC : realCC;
        Animator leaderAnim = isShadowActive ? shadowAnim : realAnim;
        CharacterController follower = isShadowActive ? realCC : shadowCC;
        Animator followerAnim = isShadowActive ? realAnim : shadowAnim;

        // 1. حركة القائد
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool run = Input.GetKey(KeyCode.LeftShift);
        bool jump = Input.GetButtonDown("Jump");

        MoveCC(leader, leaderAnim, h, v, run, jump, ref (isShadowActive ? ref shadowYV : ref realYV));

        // تشغيل صوت الخطوات للقائد
        HandleFootsteps(leader, h, v, run, isShadowActive ? shadowAudio : realAudio, ref (isShadowActive ? ref shadowStepTimer : ref realStepTimer));

        // 2. تسجيل الأوامر للتابع
        history.Enqueue(new Command { h = h, v = v, run = run, jump = jump, rot = leader.transform.rotation, time = Time.time });

        // 3. منطق التابع
        if (isRecalling && !isShadowActive)
        {
            HandleRecallLogic();
        }
        else if (isFrozen && !isShadowActive)
        {
            ApplyStaticPhysics(follower, ref shadowYV, followerAnim);
            while (history.Count > 0 && Time.time > history.Peek().time + followDelay) history.Dequeue();
        }
        else
        {
            ExecuteDelayedMovement(follower, followerAnim, ref (isShadowActive ? ref realYV : ref shadowYV));
        }
    }

    void ExecuteDelayedMovement(CharacterController cc, Animator anim, ref float yV)
    {
        while (history.Count > 0 && Time.time >= history.Peek().time + followDelay)
        {
            Command c = history.Dequeue();
            cc.transform.rotation = c.rot;
            MoveCC(cc, anim, c.h, c.v, c.run, c.jump, ref yV);

            // تشغيل صوت الخطوات للتابع
            HandleFootsteps(cc, c.h, c.v, c.run, (isShadowActive ? realAudio : shadowAudio), ref (isShadowActive ? ref realStepTimer : ref shadowStepTimer));
        }
    }

    // --- دالة الخطوات التلقائية (الجديدة) ---
    void HandleFootsteps(CharacterController cc, float h, float v, bool run, AudioSource source, ref float timer)
    {
        bool isMoving = (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f);

        if (isMoving && cc.isGrounded)
        {
            timer += Time.deltaTime;
            float interval = run ? runStepInterval : walkStepInterval;

            if (timer >= interval)
            {
                if (footstepClips.Length > 0 && source != null)
                {
                    source.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]);
                }
                timer = 0;
            }
        }
        else
        {
            timer = 0;
        }
    }

    // (بقية الدوال MoveCC, UpdateAnimator, HandleRecallLogic.. كما هي بدون تغيير في المنطق الأساسي)
    void MoveCC(CharacterController cc, Animator anim, float h, float v, bool run, bool jump, ref float yV)
    {
        if (!cc.enabled) return;
        float s = run ? runSpeed : walkSpeed;
        Vector3 m = (cc.transform.forward * v + cc.transform.right * h).normalized * s;
        if (cc.isGrounded) { yV = -2f; if (jump) { yV = jumpForce; anim.CrossFadeInFixedTime(run ? "Jump_Run" : "Jump_Up", 0.1f); } }
        else yV -= gravity * Time.deltaTime;
        m.y = yV; cc.Move(m * Time.deltaTime);
        UpdateAnimator(anim, h, v, run, cc.isGrounded);
    }

    void HandleRecallLogic()
    {
        Vector3 target = realCC.transform.position + (realCC.transform.right * 1.5f);
        if (Vector3.Distance(shadowCC.transform.position, target) > 1.5f)
        {
            Vector3 dir = (target - shadowCC.transform.position).normalized;
            shadowCC.Move(dir * 15f * Time.deltaTime);
            shadowCC.transform.rotation = Quaternion.Slerp(shadowCC.transform.rotation, realCC.transform.rotation, 10f * Time.deltaTime);
            UpdateAnimator(shadowAnim, 0, 1, true, true);
            // صوت جري أثناء الاستدعاء
            HandleFootsteps(shadowCC, 0, 1, true, shadowAudio, ref shadowStepTimer);
        }
        else { isRecalling = false; history.Clear(); }
    }

    void UpdateAnimator(Animator a, float h, float v, bool run, bool ground)
    {
        if (a == null) return;
        bool move = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;
        a.SetBool("isGrounded", ground); a.SetBool("isMoving", move);
        a.SetBool("iswalking", false); a.SetBool("isrunning", false);
        a.SetBool("isWalkingBack", false); a.SetBool("isWalkingRight", false); a.SetBool("isWalkingLeft", false);
        if (move && ground)
        {
            if (Mathf.Abs(h) > Mathf.Abs(v)) { if (h > 0.1f) a.SetBool("isWalkingRight", true); else a.SetBool("isWalkingLeft", true); }
            else { if (v < -0.1f) a.SetBool("isWalkingBack", true); else if (run && v > 0.1f) a.SetBool("isrunning", true); else a.SetBool("iswalking", true); }
        }
    }

    void ApplyStaticPhysics(CharacterController cc, ref float yV, Animator a)
    {
        yV = cc.isGrounded ? -2f : yV - gravity * Time.deltaTime;
        cc.Move(new Vector3(0, yV, 0) * Time.deltaTime);
        UpdateAnimator(a, 0, 0, false, cc.isGrounded);
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(switchKey)) { isShadowActive = !isShadowActive; history.Clear(); UpdateVisuals(); }
        if (!isShadowActive)
        {
            if (Input.GetKeyDown(freezeKey)) isFrozen = !isFrozen;
            if (Input.GetKeyDown(recallKey)) isRecalling = true;
        }
    }

    void UpdateVisuals()
    {
        if (shadowVolume) shadowVolume.weight = isShadowActive ? 1 : 0;
        if (Camera.main != null) Camera.main.cullingMask = isShadowActive ? shadowMask : realMask;
    }

    public bool IsShadowActive() { return isShadowActive; }
}