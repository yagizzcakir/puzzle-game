using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score")]
    public int CurrentScore { get; private set; }
    public int HighScore { get; private set; }
    public int MovesWithoutClear { get; private set; }

    // Events
    public event Action<int> OnScoreChanged;
    public event Action<int> OnHighScoreChanged;

    private const string HighScoreKey = "BlockBlast_HighScore";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        MovesWithoutClear = 0;
        OnScoreChanged?.Invoke(CurrentScore);
    }

    public void AddScore(int linesCleared)
    {
        if (linesCleared <= 0)
        {
            MovesWithoutClear++;
            return;
        }

        MovesWithoutClear = 0;

        int points = 0;
        switch (linesCleared)
        {
            case 1: points = 100; break;
            case 2: points = 300; break;
            default: points = 600; break; // 3+
        }

        CurrentScore += points;
        OnScoreChanged?.Invoke(CurrentScore);

        Debug.Log($"[ScoreManager] +{points} points! Total: {CurrentScore}");

        // Update high score
        if (CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            PlayerPrefs.Save();
            OnHighScoreChanged?.Invoke(HighScore);
        }
    }

    // Called when a block is placed but no lines are cleared
    public void RegisterMove()
    {
        MovesWithoutClear++;
    }
}
