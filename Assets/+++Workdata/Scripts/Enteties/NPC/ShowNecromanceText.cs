using UnityEngine;

public class ShowNecromanceText : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] canvasGroup;

    public void CanvasGroupVisibility(float canvasGroupVisibility)
    {
        foreach (var _canvasGroup in canvasGroup)
        {
            _canvasGroup.alpha = canvasGroupVisibility;
        }
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
