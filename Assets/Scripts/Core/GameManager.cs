using UnityEngine;

public enum GameState
{
    Playing,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GridSystem gridSystem;

    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        CurrentState = GameState.Playing;

        if (gridSystem != null)
            gridSystem.InitializeGrid();
        else
            Debug.LogError("[GameManager] GridSystem reference is missing!");

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetScore();

        AdjustCamera();

        Debug.Log("[GameManager] Game initialized!");
    }

    private void AdjustCamera()
    {
        Camera cam = Camera.main;
        if (cam == null || gridSystem == null) return;

        float gridHeight = gridSystem.Rows * (gridSystem.CellSize + gridSystem.CellSpacing);
        float gridWidth = gridSystem.Cols * (gridSystem.CellSize + gridSystem.CellSpacing);

        float screenRatio = (float)Screen.width / Screen.height;
        float targetRatio = gridWidth / gridHeight;
        float verticalPadding = 4.0f;

        if (screenRatio >= targetRatio)
            cam.orthographicSize = (gridHeight + verticalPadding) / 2f;
        else
            cam.orthographicSize = (gridWidth / screenRatio + verticalPadding) / 2f;

        cam.transform.position = new Vector3(0, 1.0f, -10f);
        cam.backgroundColor = new Color(0.08f, 0.08f, 0.12f);
    }

    public void OnBlockPlaced()
    {
        if (CurrentState != GameState.Playing) return;

        int linesCleared = gridSystem.ClearFullLines();

        if (ScoreManager.Instance != null)
        {
            if (linesCleared > 0)
                ScoreManager.Instance.AddScore(linesCleared);
            else
                ScoreManager.Instance.RegisterMove();
        }
    }

    public void GameOver()
    {
        CurrentState = GameState.GameOver;
        Debug.Log("[GameManager] Game Over! Score: " + (ScoreManager.Instance != null ? ScoreManager.Instance.CurrentScore : 0));
        
        if (UIManager.Instance != null && ScoreManager.Instance != null)
            UIManager.Instance.ShowGameOver(ScoreManager.Instance.CurrentScore);
    }

    public void RestartGame()
    {
        foreach (Transform child in gridSystem.transform)
            Destroy(child.gameObject);

        if (UIManager.Instance != null)
            UIManager.Instance.HideGameOver();

        if (BlockSpawner.Instance != null)
            BlockSpawner.Instance.ClearAllBlocks();

        StartGame();

        if (BlockSpawner.Instance != null)
            BlockSpawner.Instance.SpawnNewSet(); // Trigger first set
    }
}
