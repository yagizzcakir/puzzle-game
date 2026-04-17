using UnityEngine;
using System.Collections.Generic;

public class BlockPiece : MonoBehaviour
{
    [Header("Block Data")]
    public int ColorId { get; private set; }
    public int[,] ShapeMatrix { get; private set; }
    public bool IsUsed { get; private set; }

    private List<GameObject> cellVisuals = new List<GameObject>();
    private Sprite cellSprite;
    private Color blockColor;

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private bool isDragging = false;

    // Scale when in the spawn panel (smaller)
    private float panelScale = 0.5f;
    // Scale when dragging (full size to match grid)
    private float dragScale = 1.0f;

    public void Initialize(int[,] shape, int colorId, Sprite sprite)
    {
        ShapeMatrix = shape;
        ColorId = colorId;
        cellSprite = sprite;
        IsUsed = false;

        blockColor = GridSystem.BlockColors[(colorId - 1) % GridSystem.BlockColors.Length];

        BuildVisual();
    }

    private void BuildVisual()
    {
        // Clear old visuals
        foreach (var cell in cellVisuals)
        {
            if (cell != null) Destroy(cell);
        }
        cellVisuals.Clear();

        int rows = ShapeMatrix.GetLength(0);
        int cols = ShapeMatrix.GetLength(1);

        // Center the shape
        float offsetX = -(cols - 1) / 2f;
        float offsetY = (rows - 1) / 2f;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (ShapeMatrix[r, c] == 1)
                {
                    GameObject cellObj = new GameObject($"BlockCell_{r}_{c}");
                    cellObj.transform.SetParent(transform);
                    cellObj.transform.localPosition = new Vector3(
                        offsetX + c,
                        offsetY - r,
                        0
                    );
                    cellObj.transform.localScale = new Vector3(0.9f, 0.9f, 1f);

                    SpriteRenderer sr = cellObj.AddComponent<SpriteRenderer>();
                    sr.sprite = cellSprite;
                    sr.color = blockColor;
                    sr.sortingOrder = 10;

                    cellVisuals.Add(cellObj);
                }
            }
        }

        // Set initial scale for panel display
        transform.localScale = Vector3.one * panelScale;
    }

    public void SetPanelPosition(Vector3 position)
    {
        originalPosition = position;
        transform.position = position;
        originalScale = Vector3.one * panelScale;
        transform.localScale = originalScale;
    }

    public void StartDrag()
    {
        if (IsUsed) return;
        isDragging = true;
        transform.localScale = Vector3.one * dragScale;

        // Raise sorting order while dragging
        foreach (var cell in cellVisuals)
        {
            var sr = cell.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = 20;
        }
    }

    public void UpdateDragPosition(Vector3 worldPos)
    {
        if (!isDragging) return;
        // Offset upward so the block is visible above the finger
        transform.position = worldPos + Vector3.up * 1.5f;
    }

    public void EndDrag(bool placed)
    {
        isDragging = false;

        if (placed)
        {
            MarkAsUsed();
        }
        else
        {
            // Return to original position
            transform.position = originalPosition;
            transform.localScale = originalScale;

            // Reset sorting order
            foreach (var cell in cellVisuals)
            {
                var sr = cell.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sortingOrder = 10;
            }
        }
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
        // Fade out
        foreach (var cell in cellVisuals)
        {
            var sr = cell.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color;
                c.a = 0.2f;
                sr.color = c;
            }
        }
        transform.localScale = originalScale;
        transform.position = originalPosition;
    }

    public string GetShapeString()
    {
        return BlockShapes.ShapeToString(ShapeMatrix);
    }

    // Get the grid-aligned top-left position for placement
    public Vector2Int GetGridPlacementOffset()
    {
        int rows = ShapeMatrix.GetLength(0);
        int cols = ShapeMatrix.GetLength(1);
        return new Vector2Int(-(rows - 1) / 2, -(cols - 1) / 2);
    }
}
