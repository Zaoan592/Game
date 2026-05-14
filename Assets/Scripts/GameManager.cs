using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Scoring System")]
    public int score = 0;
    public int totalCollectibles = 5;
    public TextMeshProUGUI scoreText;

    [Header("UI Panels")]
    public GameObject gameOverPanel; // 死亡面板
    public GameObject victoryPanel;  // 获胜面板

    [Header("Level Progression")]
    public GameObject exitDoor;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip victorySound;   // 胜利音效

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 游戏开始，恢复时间流速，隐藏所有面板
        Time.timeScale = 1f;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (exitDoor != null) exitDoor.SetActive(false);

        UpdateUI();
    }

    // --- 核心逻辑：增加分数 ---
    public void AddScore()
    {
        score++;
        UpdateUI();

        if (score >= totalCollectibles)
        {
            if (exitDoor != null)
            {
                exitDoor.SetActive(true);
            }
        }
    }

    // --- 核心逻辑：显示死亡界面 (MonsterAI 调用的就是这里！) ---
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f; // 时间静止
            UnlockCursor();
        }
    }

    // --- 核心逻辑：显示获胜界面 ---
    public void ShowVictory()
    {
        if (victoryPanel != null)
        {
            // 播放胜利音效
            if (audioSource != null && victorySound != null)
            {
                audioSource.PlayOneShot(victorySound);
            }

            victoryPanel.SetActive(true);
            Time.timeScale = 0f; // 时间静止
            UnlockCursor();
        }
    }

    // --- 通用工具：解锁鼠标 ---
    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // --- 按钮功能：重新开始 ---
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- 按钮功能：返回菜单 ---
    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }

    void UpdateUI()
    {
        if (scoreText != null)
            if (score < totalCollectibles)
            {
                scoreText.text = "Collected: " + score + " / " + totalCollectibles;
            }
            else
            {
                // 收集齐后，左上角的文字变成红色警告！
                scoreText.text = "Exit Opened! ESCAPE!";
                scoreText.color = Color.red;
            }
    }
}