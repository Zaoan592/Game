using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;      // 走路速度
    public float runSpeed = 6f;       // 奔跑速度
    public float mouseSensitivity = 100f;
    private float currentSpeed;       // 当前实际速度

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float xRotation = 0f;

    [Header("References")]
    public Transform cameraTransform;
    private Camera playerCamera;      // 用于控制FOV

    [Header("Stamina System)")]
    public float maxStamina = 100f;   // 最大体力值
    public float currentStamina;      // 当前体力值
    public float drainRate = 25f;     // 每秒消耗体力的速度
    public float regenRate = 15f;     // 每秒恢复体力的速度
    public Image staminaBarFill;

    [Header("Exhaustion Penalty")]
    public bool isExhausted = false;          // 是否处于虚脱状态
    public float recoveryThreshold = 30f;     // 需要恢复到多少才能重新跑 (比如 30)
    public Color normalStaminaColor = Color.yellow; // 正常的体力条颜色
    public Color exhaustedColor = Color.red;        // 虚脱时的体力条颜色

    [Header("Audio-Visual Feedback ")]
    public float normalFOV = 60f;             // 正常走路的视野
    public float runFOV = 75f;                // 奔跑时的视野 (产生速度感)
    public float fovTransitionSpeed = 5f;     // 视野变化的平滑速度

    public AudioSource audioSource;           // 播放心跳/喘息的组件
    public AudioClip breathingSound;          // 喘息音效

    void Start()
    {
        // 初始化
        currentStamina = maxStamina;
        currentSpeed = walkSpeed;

        controller = GetComponent<CharacterController>();
        playerCamera = cameraTransform.GetComponent<Camera>(); // 获取摄像机组件

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    void Update()
    {
        // ==========================================
        // 1. 体力、疲劳与奔跑逻辑
        // ==========================================
        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        bool isHoldingShift = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;

        // 疲劳状态判断逻辑：如果体力耗尽（<=0），进入虚脱状态
        if (currentStamina <= 0)
        {
            isExhausted = true;
        }
        // 如果处于虚脱状态，必须恢复到阈值（recoveryThreshold）才能解除
        else if (isExhausted && currentStamina >= recoveryThreshold)
        {
            isExhausted = false;
        }

        // 只有当按住Shift、正在移动，且【没有处于虚脱状态】时，才能奔跑
        bool isTryingToRun = isHoldingShift && isMoving && !isExhausted;

        if (isTryingToRun && currentStamina > 0)
        {
            currentSpeed = runSpeed;
            currentStamina -= drainRate * Time.deltaTime;
        }
        else
        {
            currentSpeed = walkSpeed;
            if (currentStamina < maxStamina)
            {
                currentStamina += regenRate * Time.deltaTime;
            }
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        // 更新 UI 体力条的显示长度和颜色
        if (staminaBarFill != null)
        {
            staminaBarFill.fillAmount = currentStamina / maxStamina;
            // 处于虚脱状态时变为红色警告，正常时为普通颜色
            staminaBarFill.color = isExhausted ? exhaustedColor : normalStaminaColor;
        }

        // ==========================================
        // 2. 动态视野与喘息音效反馈
        // ==========================================

        // 动态 FOV：奔跑时拉大视野，走路时恢复正常
        if (playerCamera != null)
        {
            float targetFOV = isTryingToRun ? runFOV : normalFOV;
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * fovTransitionSpeed);
        }

        // 喘息音效：当体力低于 20% 时播放，恢复后停止
        if (audioSource != null && breathingSound != null)
        {
            if (currentStamina <= maxStamina * 0.2f && !audioSource.isPlaying)
            {
                audioSource.clip = breathingSound;
                audioSource.Play();
            }
            else if (currentStamina > maxStamina * 0.2f && audioSource.isPlaying && audioSource.clip == breathingSound)
            {
                audioSource.Stop();
            }
        }

        // ==========================================
        // 3. 玩家移动逻辑
        // ==========================================
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // ==========================================
        // 4. 玩家视角逻辑
        // ==========================================
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}