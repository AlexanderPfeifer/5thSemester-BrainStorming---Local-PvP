using UnityEngine;
using UnityEngine.UI;

public class ShowNecromanceText : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] canvasGroup;

    public void CanvasGroupVisibility(float canvasGroupVisibility)
    {
        foreach (var _canvasGroup in canvasGroup)
        {
            _canvasGroup.alpha = canvasGroupVisibility;
            if(canvasGroupVisibility <= 0) 
                _canvasGroup.GetComponentInChildren<Image>().fillAmount = 0;
        }
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
