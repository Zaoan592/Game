using UnityEngine;
using TMPro; // 引入 TextMeshPro 命名空间

public class FPSCounter : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI fpsText; // 把你的 FPSText 拖到这里

    [Header("Settings")]
    private float updateTimer = 0f;
    public float refreshRate = 0.5f; // 每 0.5 秒刷新一次显示，防止数字闪烁太快看不清

    void Update()
    {
        // 确保 UI 文本已经赋值
        if (fpsText != null)
        {
            // 计时器控制刷新频率
            if (Time.unscaledTime > updateTimer)
            {
                // 核心计算逻辑：1 除以 (完成上一帧所花费的时间)
                // 注意：这里使用 unscaledDeltaTime 而不是 deltaTime
                float currentFps = 1f / Time.unscaledDeltaTime;

                // 将浮点数转换为整数并显示
                fpsText.text = "FPS: " + Mathf.RoundToInt(currentFps);

                // 重置计时器
                updateTimer = Time.unscaledTime + refreshRate;
            }
        }
    }
}