using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int score = 0;
    public int totalCollectibles = 5;

    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddScore()
    {
        score++;
        UpdateUI();

        if (score >= totalCollectibles)
        {
            Debug.Log("You Win!");
        }
    }

    void UpdateUI()
    {
        scoreText.text = "Collected: " + score + "/" + totalCollectibles;
    }
}
