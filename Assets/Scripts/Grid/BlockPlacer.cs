using UnityEngine;
using UnityEngine.InputSystem;

public class BlockPlacer : MonoBehaviour
{
    public static BlockPlacer Instance { get; private set; }

    [Header("Preview Colors")]
    [SerializeField] private Color validPreviewColor = new Color(0.3f, 0.8f, 0.3f, 0.4f);
    [SerializeField] private Color invalidPreviewColor = new Color(0.9f, 0.2f, 0.2f, 0.3f);

    private BlockPiece selectedBlock;
    private bool isDragging = false;
    private Camera mainCamera;

    // Current preview state
    private Vector2Int lastPreviewPos = new Vector2Int(-1, -1);
    private bool lastPreviewValid = false;

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
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            return;

        HandleInput();
    }

    private void HandleInput()
    {
        var pointer = Pointer.current;
        if (pointer == null) return;

        Vector2 pointerPos = pointer.position.ReadValue();

        // Start Drag
        if (pointer.press.wasPressedThisFrame)
        {
            TrySelectBlock(pointerPos);
        }
        
        // Handle dragging state
        if (isDragging && selectedBlock != null)
        {
            if (pointer.press.isPressed)
            {
                UpdateDrag(pointerPos);
            }
            else
            {
                // Fallback: If not pressed but still marked as dragging, place it
                TryPlaceBlock();
            }
        }
    }

    private void TrySelectBlock(Vector2 screenPos)
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(screenPos);
        mouseWorldPos.z = 0;

        // Check if we clicked on a block piece
        var blocks = BlockSpawner.Instance.GetCurrentBlocks();
        float closestDist = float.MaxValue;
        BlockPiece closestBlock = null;

        foreach (var block in blocks)
        {
            if (block == null || block.IsUsed) continue;

            float dist = Vector3.Distance(mouseWorldPos, block.transform.position);
            if (dist < 1.5f && dist < closestDist)
            {
                closestDist = dist;
                closestBlock = block;
            }
        }

        if (closestBlock != null)
        {
            selectedBlock = closestBlock;
            isDragging = true;
            selectedBlock.StartDrag();
        }
    }

    private void UpdateDrag(Vector2 screenPos)
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(screenPos);
        mouseWorldPos.z = 0;

        selectedBlock.UpdateDragPosition(mouseWorldPos);

        // Show preview on grid
        ShowPreview(mouseWorldPos);
    }

    private void ShowPreview(Vector3 worldPos)
    {
        // Offset up to match block visual offset
        Vector3 adjustedPos = worldPos + Vector3.up * 1.5f;

        GridSystem grid = GridSystem.Instance;
        if (grid == null) return;

        Vector2Int gridPos = grid.WorldToGridPosition(adjustedPos);

        // Only update preview if position changed
        if (gridPos == lastPreviewPos) return;

        // Clear old preview
        grid.ClearAllHighlights();
        lastPreviewPos = gridPos;

        // Adjust grid position for shape offset
        int shapeRows = selectedBlock.ShapeMatrix.GetLength(0);
        int shapeCols = selectedBlock.ShapeMatrix.GetLength(1);
        int startRow = gridPos.x - shapeRows / 2;
        int startCol = gridPos.y - shapeCols / 2;

        bool canPlace = grid.CanPlaceBlock(startRow, startCol, selectedBlock.ShapeMatrix);
        lastPreviewValid = canPlace;

        Color previewColor = canPlace ? validPreviewColor : invalidPreviewColor;
        grid.HighlightCells(startRow, startCol, selectedBlock.ShapeMatrix, previewColor);
    }

    private void TryPlaceBlock()
    {
        GridSystem grid = GridSystem.Instance;
        grid.ClearAllHighlights();

        if (selectedBlock != null && lastPreviewValid)
        {
            int shapeRows = selectedBlock.ShapeMatrix.GetLength(0);
            int shapeCols = selectedBlock.ShapeMatrix.GetLength(1);
            int startRow = lastPreviewPos.x - shapeRows / 2;
            int startCol = lastPreviewPos.y - shapeCols / 2;

            // Place the block
            bool placed = grid.PlaceBlock(startRow, startCol, selectedBlock.ShapeMatrix, selectedBlock.ColorId);

            if (placed)
            {
                selectedBlock.EndDrag(true);
                
                // Check for line clears
                int linesCleared = grid.ClearFullLines();
                
                // Update score
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddScore(linesCleared);
                }

                // Check if we need to spawn new blocks
                BlockSpawner.Instance.CheckAndRefresh();
                
                // Check Game Over
                CheckGameOver();
            }
            else
            {
                selectedBlock.EndDrag(false);
            }
        }
        else if (selectedBlock != null)
        {
            selectedBlock.EndDrag(false);
        }

        // Reset state
        selectedBlock = null;
        isDragging = false;
        lastPreviewPos = new Vector2Int(-1, -1);
        lastPreviewValid = false;
    }

    private void CheckGameOver()
    {
        if (GridSystem.Instance == null || BlockSpawner.Instance == null) return;

        var availableShapes = BlockSpawner.Instance.GetAvailableShapes();
        if (availableShapes.Count == 0) return; // All used, new set coming

        bool canPlaceAny = GridSystem.Instance.CanAnyBlockBePlaced(availableShapes);
        if (!canPlaceAny)
        {
            GameManager.Instance.GameOver();
        }
    }
}
