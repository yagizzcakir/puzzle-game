using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void Start()
    {
        // Subscribe to score events
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += UpdateScoreUI;
            ScoreManager.Instance.OnHighScoreChanged += UpdateHighScoreUI;
            
            // Initial update
            UpdateScoreUI(ScoreManager.Instance.CurrentScore);
            UpdateHighScoreUI(ScoreManager.Instance.HighScore);
        }
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateScoreUI;
            ScoreManager.Instance.OnHighScoreChanged -= UpdateHighScoreUI;
        }
    }

    public void ShowGameOver(int score)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null)
                finalScoreText.text = $"SCORE: {score}";

            // Ensure the button is selected and interactable
            var btn = gameOverPanel.GetComponentInChildren<UnityEngine.UI.Button>();
            if (btn != null)
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(btn.gameObject);
                btn.interactable = true;
            }
        }
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void UpdateScoreUI(int score)
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    private void UpdateHighScoreUI(int highScore)
    {
        if (highScoreText != null)
            highScoreText.text = $"BEST\n{highScore}";
    }
}
