using UnityEngine;
using System.Collections.Generic;

public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance { get; private set; }

    [Header("Grid Settings")]
    public int Rows = 8;
    public int Cols = 8;
    public float CellSize = 1.0f;
    public float CellSpacing = 0.08f;

    [Header("References")]
    [SerializeField] private GameObject cellPrefab;

    // Grid data: 0 = empty, 1+ = color ID
    private int[,] grid;
    private Cell[,] cells;

    // Block colors
    public static readonly Color[] BlockColors = new Color[]
    {
        new Color(0.204f, 0.596f, 0.859f), // Blue
        new Color(0.906f, 0.298f, 0.235f), // Red
        new Color(0.180f, 0.800f, 0.443f), // Green
        new Color(0.608f, 0.349f, 0.714f), // Purple
        new Color(0.945f, 0.769f, 0.059f), // Yellow
        new Color(0.902f, 0.494f, 0.133f), // Orange
        new Color(0.102f, 0.737f, 0.612f), // Teal
        new Color(0.925f, 0.412f, 0.608f), // Pink
    };

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
    }

    public void InitializeGrid()
    {
        grid = new int[Rows, Cols];
        cells = new Cell[Rows, Cols];

        // Calculate offset to center the grid at (0,0)
        float totalWidth = Cols * (CellSize + CellSpacing) - CellSpacing;
        float totalHeight = Rows * (CellSize + CellSpacing) - CellSpacing;
        float startX = -totalWidth / 2f + CellSize / 2f;
        float startY = totalHeight / 2f - CellSize / 2f;

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                float x = startX + c * (CellSize + CellSpacing);
                float y = startY - r * (CellSize + CellSpacing);

                GameObject cellObj = Instantiate(cellPrefab, transform);
                cellObj.name = $"Cell_{r}_{c}";
                cellObj.transform.localPosition = new Vector3(x, y, 0);
                cellObj.transform.localRotation = Quaternion.identity;

                Cell cell = cellObj.GetComponent<Cell>();
                cell.Initialize(r, c);

                cells[r, c] = cell;
                grid[r, c] = 0;
            }
        }

        Debug.Log($"[GridSystem] {Rows}x{Cols} grid initialized.");
    }

    // Check if a block shape can be placed at the given position
    public bool CanPlaceBlock(int row, int col, int[,] shape)
    {
        int shapeRows = shape.GetLength(0);
        int shapeCols = shape.GetLength(1);

        for (int r = 0; r < shapeRows; r++)
        {
            for (int c = 0; c < shapeCols; c++)
            {
                if (shape[r, c] == 1)
                {
                    int gridRow = row + r;
                    int gridCol = col + c;

                    // Check bounds
                    if (gridRow < 0 || gridRow >= Rows || gridCol < 0 || gridCol >= Cols)
                        return false;

                    // Check if cell is already occupied
                    if (grid[gridRow, gridCol] != 0)
                        return false;
                }
            }
        }

        return true;
    }

    // Place a block on the grid
    public bool PlaceBlock(int row, int col, int[,] shape, int colorId)
    {
        if (!CanPlaceBlock(row, col, shape))
            return false;

        int shapeRows = shape.GetLength(0);
        int shapeCols = shape.GetLength(1);

        Color blockColor = BlockColors[(colorId - 1) % BlockColors.Length];

        for (int r = 0; r < shapeRows; r++)
        {
            for (int c = 0; c < shapeCols; c++)
            {
                if (shape[r, c] == 1)
                {
                    int gridRow = row + r;
                    int gridCol = col + c;

                    grid[gridRow, gridCol] = colorId;
                    cells[gridRow, gridCol].SetColor(blockColor);
                }
            }
        }

        return true;
    }

    // Check and clear full rows and columns, returns number of lines cleared
    public int ClearFullLines()
    {
        List<int> fullRows = new List<int>();
        List<int> fullCols = new List<int>();

        // Check rows
        for (int r = 0; r < Rows; r++)
        {
            bool full = true;
            for (int c = 0; c < Cols; c++)
            {
                if (grid[r, c] == 0)
                {
                    full = false;
                    break;
                }
            }
            if (full) fullRows.Add(r);
        }

        // Check columns
        for (int c = 0; c < Cols; c++)
        {
            bool full = true;
            for (int r = 0; r < Rows; r++)
            {
                if (grid[r, c] == 0)
                {
                    full = false;
                    break;
                }
            }
            if (full) fullCols.Add(c);
        }

        // Clear all full rows
        foreach (int r in fullRows)
        {
            ClearRow(r);
        }

        // Clear all full columns
        foreach (int c in fullCols)
        {
            ClearColumn(c);
        }

        int totalCleared = fullRows.Count + fullCols.Count;

        if (totalCleared > 0)
            Debug.Log($"[GridSystem] Cleared {fullRows.Count} rows, {fullCols.Count} columns.");

        return totalCleared;
    }

    private void ClearRow(int row)
    {
        for (int c = 0; c < Cols; c++)
        {
            if (grid[row, c] != 0)
            {
                grid[row, c] = 0;
                cells[row, c].AnimateClear();
            }
        }
    }

    private void ClearColumn(int col)
    {
        for (int r = 0; r < Rows; r++)
        {
            if (grid[r, col] != 0)
            {
                grid[r, col] = 0;
                cells[r, col].AnimateClear();
            }
        }
    }

    // Highlight cells for block preview
    public void HighlightCells(int row, int col, int[,] shape, Color color)
    {
        int shapeRows = shape.GetLength(0);
        int shapeCols = shape.GetLength(1);

        for (int r = 0; r < shapeRows; r++)
        {
            for (int c = 0; c < shapeCols; c++)
            {
                if (shape[r, c] == 1)
                {
                    int gridRow = row + r;
                    int gridCol = col + c;

                    if (gridRow >= 0 && gridRow < Rows && gridCol >= 0 && gridCol < Cols)
                    {
                        cells[gridRow, gridCol].SetHighlight(color);
                    }
                }
            }
        }
    }

    // Clear all highlights
    public void ClearAllHighlights()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                cells[r, c].ClearHighlight();
            }
        }
    }

    // Get grid snapshot for AI
    public int[,] GetGridSnapshot()
    {
        int[,] snapshot = new int[Rows, Cols];
        System.Array.Copy(grid, snapshot, grid.Length);
        return snapshot;
    }

    // Get the world position of a specific cell
    public Vector3 GetCellWorldPosition(int row, int col)
    {
        if (row >= 0 && row < Rows && col >= 0 && col < Cols)
        {
            return cells[row, col].transform.position;
        }
        return Vector3.zero;
    }

    // Convert world position to grid coordinates
    public Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        Vector3 localPos = transform.InverseTransformPoint(worldPos);

        float totalWidth = Cols * (CellSize + CellSpacing) - CellSpacing;
        float totalHeight = Rows * (CellSize + CellSpacing) - CellSpacing;
        float startX = -totalWidth / 2f + CellSize / 2f;
        float startY = totalHeight / 2f - CellSize / 2f;

        int col = Mathf.RoundToInt((localPos.x - (startX - CellSize / 2f)) / (CellSize + CellSpacing));
        int row = Mathf.RoundToInt(((startY + CellSize / 2f) - localPos.y) / (CellSize + CellSpacing));

        return new Vector2Int(row, col);
    }

    // Check if any block from a list can be placed anywhere on the grid
    public bool CanAnyBlockBePlaced(List<int[,]> shapes)
    {
        foreach (var shape in shapes)
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (CanPlaceBlock(r, c, shape))
                        return true;
                }
            }
        }
        return false;
    }

    public Cell GetCell(int row, int col)
    {
        if (row >= 0 && row < Rows && col >= 0 && col < Cols)
            return cells[row, col];
        return null;
    }
}
