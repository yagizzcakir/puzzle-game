using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleRestart : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[SimpleRestart] Manual click detected via IPointerClickHandler!");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
}
