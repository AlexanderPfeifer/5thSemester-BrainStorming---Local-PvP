using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PointsVisualization : MonoBehaviour
{
    [SerializeField] private RectTransform sliderHandle;
    [SerializeField] private Vector2 xPosition;
    [SerializeField] private float targetFontSize;
    private float initialSize;

    private void Start()
    {
        GetComponent<TextMeshProUGUI>().enabled = false;

        initialSize = GetComponent<TextMeshProUGUI>().fontSize;
    }

    public IEnumerator ShowPlusIcon()
    {
        GetComponent<TextMeshProUGUI>().enabled = true;

        RectTransform _rectTransform = GetComponent<RectTransform>();

        _rectTransform.localPosition = new Vector3(
            Random.Range(xPosition.x, xPosition.y),
            sliderHandle.transform.localPosition.y,
            _rectTransform.localPosition.z
        );

        float _duration = .5f;
        float _elapsedTime = 0f;

        while (_elapsedTime < _duration)
        {
            _elapsedTime += Time.deltaTime;

            float _newSize = Mathf.Lerp(initialSize, targetFontSize, _elapsedTime / _duration);
            GetComponent<TextMeshProUGUI>().fontSize = _newSize;

            yield return null;
        }

        GetComponent<TextMeshProUGUI>().fontSize = targetFontSize;
        
        GetComponent<TextMeshProUGUI>().enabled = false;
    }
}
