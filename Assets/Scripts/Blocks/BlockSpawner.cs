using UnityEngine;
using System.Collections.Generic;

public class BlockSpawner : MonoBehaviour
{
    public static BlockSpawner Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float blockSpacing = 3.0f;
    [SerializeField] private float panelY = -4.5f;

    // Current set of 3 blocks
    private List<BlockPiece> currentBlocks = new List<BlockPiece>();
    private Sprite cellSprite;

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
        // Load the cell sprite
        cellSprite = Resources.Load<Sprite>("WhiteSquare");
        if (cellSprite == null)
        {
            // Try to load from Assets/Sprites
            var tex = UnityEngine.Resources.Load<Texture2D>("WhiteSquare");
            if (tex == null)
            {
                // Create a simple white sprite programmatically
                cellSprite = CreateDefaultSprite();
            }
        }

        SpawnNewSet();
    }

    private Sprite CreateDefaultSprite()
    {
        Texture2D tex = new Texture2D(32, 32);
        Color[] colors = new Color[32 * 32];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = Color.white;
        tex.SetPixels(colors);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
    }

    public void SpawnNewSet()
    {
        // Destroy old blocks
        foreach (var block in currentBlocks)
        {
            if (block != null)
                Destroy(block.gameObject);
        }
        currentBlocks.Clear();

        // Spawn 3 new blocks
        for (int i = 0; i < 3; i++)
        {
            int[,] shape = BlockShapes.GetRandomShape();
            int colorId = Random.Range(1, GridSystem.BlockColors.Length + 1);

            GameObject blockObj = new GameObject($"Block_{i}");
            blockObj.transform.SetParent(transform);

            BlockPiece piece = blockObj.AddComponent<BlockPiece>();
            piece.Initialize(shape, colorId, cellSprite);

            // Position in panel: 3 blocks side by side
            float xPos = (i - 1) * blockSpacing;
            piece.SetPanelPosition(new Vector3(xPos, panelY, 0));

            currentBlocks.Add(piece);
        }

        Debug.Log("[BlockSpawner] New block set spawned.");
    }

    public bool AllBlocksUsed()
    {
        foreach (var block in currentBlocks)
        {
            if (block != null && !block.IsUsed)
                return false;
        }
        return true;
    }

    public void ClearAllBlocks()
    {
        foreach (var block in currentBlocks)
        {
            if (block != null)
                Destroy(block.gameObject);
        }
        currentBlocks.Clear();

        // Safety: clear any other children
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void CheckAndRefresh()
    {
        if (AllBlocksUsed())
        {
            SpawnNewSet();
        }
    }

    public List<BlockPiece> GetCurrentBlocks()
    {
        return currentBlocks;
    }

    public List<int[,]> GetAvailableShapes()
    {
        var shapes = new List<int[,]>();
        foreach (var block in currentBlocks)
        {
            if (block != null && !block.IsUsed)
            {
                shapes.Add(block.ShapeMatrix);
            }
        }
        return shapes;
    }

    public void SetCellSprite(Sprite sprite)
    {
        cellSprite = sprite;
    }
}
