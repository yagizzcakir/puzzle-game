using UnityEngine;

public static class BlockShapes
{
    // All block shape definitions
    // 1 = filled cell, 0 = empty cell
    public static readonly int[][,] All = new int[][,]
    {
        // Single (1x1)
        new int[,] { {1} },

        // Horizontal 2 (1x2)
        new int[,] { {1, 1} },

        // Horizontal 3 (1x3)
        new int[,] { {1, 1, 1} },

        // Horizontal 4 (1x4)
        new int[,] { {1, 1, 1, 1} },

        // Horizontal 5 (1x5)
        new int[,] { {1, 1, 1, 1, 1} },

        // Vertical 2 (2x1)
        new int[,] { {1}, {1} },

        // Vertical 3 (3x1)
        new int[,] { {1}, {1}, {1} },

        // Vertical 4 (4x1)
        new int[,] { {1}, {1}, {1}, {1} },

        // Vertical 5 (5x1)
        new int[,] { {1}, {1}, {1}, {1}, {1} },

        // Square 2x2
        new int[,] { {1, 1}, {1, 1} },

        // Square 3x3
        new int[,] { {1, 1, 1}, {1, 1, 1}, {1, 1, 1} },

        // L shape
        new int[,] { {1, 0}, {1, 0}, {1, 1} },

        // J shape
        new int[,] { {0, 1}, {0, 1}, {1, 1} },

        // L shape rotated
        new int[,] { {1, 1}, {1, 0}, {1, 0} },

        // J shape rotated
        new int[,] { {1, 1}, {0, 1}, {0, 1} },

        // T shape
        new int[,] { {1, 1, 1}, {0, 1, 0} },

        // T shape inverted
        new int[,] { {0, 1, 0}, {1, 1, 1} },

        // S shape
        new int[,] { {0, 1, 1}, {1, 1, 0} },

        // Z shape
        new int[,] { {1, 1, 0}, {0, 1, 1} },

        // L long
        new int[,] { {1, 1, 1}, {1, 0, 0} },

        // L long variant
        new int[,] { {1, 0, 0}, {1, 1, 1} },

        // Corner 2x2
        new int[,] { {1, 1}, {1, 0} },

        // Corner 2x2 variant
        new int[,] { {1, 1}, {0, 1} },

        // Corner 2x2 variant
        new int[,] { {0, 1}, {1, 1} },

        // Corner 2x2 variant
        new int[,] { {1, 0}, {1, 1} },
    };

    // Get a random shape
    public static int[,] GetRandomShape()
    {
        int index = Random.Range(0, All.Length);
        return All[index];
    }

    // Get the shape as a string representation for AI
    public static string ShapeToString(int[,] shape)
    {
        int rows = shape.GetLength(0);
        int cols = shape.GetLength(1);
        var sb = new System.Text.StringBuilder();
        sb.Append("[");
        for (int r = 0; r < rows; r++)
        {
            if (r > 0) sb.Append("|");
            for (int c = 0; c < cols; c++)
            {
                sb.Append(shape[r, c] == 1 ? "#" : ".");
            }
        }
        sb.Append("]");
        return sb.ToString();
    }
}
