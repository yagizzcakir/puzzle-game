using UnityEngine;

public class Cell : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    public int Row { get; private set; }
    public int Col { get; private set; }
    public bool IsOccupied { get; private set; }

    private Color emptyColor = new Color(0.15f, 0.15f, 0.2f, 1f);
    private Color currentColor;

    public void Initialize(int row, int col)
    {
        Row = row;
        Col = col;
        IsOccupied = false;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        SetEmpty();
    }

    public void SetColor(Color color)
    {
        StopAllCoroutines();
        currentColor = color;
        spriteRenderer.color = color;
        spriteRenderer.transform.localScale = Vector3.one * 0.95f;
        IsOccupied = true;
    }

    public void SetEmpty()
    {
        StopAllCoroutines();
        currentColor = emptyColor;
        spriteRenderer.color = emptyColor;
        spriteRenderer.transform.localScale = Vector3.one * 0.95f;
        IsOccupied = false;
    }

    public void AnimateClear()
    {
        StartCoroutine(ClearRoutine());
    }

    private System.Collections.IEnumerator ClearRoutine()
    {
        float duration = 0.3f;
        float elapsed = 0f;
        Color startColor = currentColor;
        Color flashColor = Color.white;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Flash and fade out
            spriteRenderer.color = Color.Lerp(flashColor, emptyColor, t);
            // Shrink effect
            spriteRenderer.transform.localScale = Vector3.Lerp(Vector3.one * 1.1f, Vector3.zero, t);
            
            yield return null;
        }

        SetEmpty();
    }

    public void SetHighlight(Color color)
    {
        if (!IsOccupied)
        {
            spriteRenderer.color = color;
        }
    }

    public void ClearHighlight()
    {
        spriteRenderer.color = IsOccupied ? currentColor : emptyColor;
    }

    public Color GetCurrentColor()
    {
        return currentColor;
    }
}
