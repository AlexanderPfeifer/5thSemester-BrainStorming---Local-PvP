using System.Collections;
using TMPro;
using UnityEngine;

public class HudUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] countDownText;
    private float initialFontSize;

    void OnEnable()
    {
        PlayerRegistryManager.Instance.AllPlayersReady += StartGameCountdown;
    }
    
    void OnDisable()
    {
        PlayerRegistryManager.Instance.AllPlayersReady -= StartGameCountdown;
    }

    private void StartGameCountdown()
    {
        initialFontSize = countDownText[0].fontSize;
        StartCoroutine(CountDownCoroutine());
    }

    private IEnumerator CountDownCoroutine()
    {
        float _duration = 1f; 
        float _elapsedTime = 0f;
        float _targetFontSize = 220f;

        for (int _i = 3; _i > 0; _i--)
        {
            foreach (var _countDownText in countDownText)
            {
                _countDownText.text = _i.ToString();
                _countDownText.fontSize = initialFontSize;
                _elapsedTime = 0;
            }
            
            while (_elapsedTime < _duration)
            {
                _elapsedTime += Time.deltaTime;
                float _newFontSize = Mathf.Lerp(initialFontSize, _targetFontSize, _elapsedTime / _duration);

                foreach (var _countDownText in countDownText)
                {
                    _countDownText.fontSize = Mathf.RoundToInt(_newFontSize); 
                }

                yield return null; 
            }
            
            foreach (var _countDownText in countDownText)
            {
                _countDownText.fontSize = Mathf.RoundToInt(_targetFontSize);
            }
        }
        
        _duration = 2f;
        _elapsedTime = 0f;
        
        //Changed the font size here because the text is too big and would look weird when scaling
        foreach (var _countDownText in countDownText)
        {
            _countDownText.text = "BRING \n ZOMBIES \n TO THE \n BRAIN!";
            _countDownText.fontSize = 100;
            _targetFontSize = 140f;
            initialFontSize = 100f;
        }
        
        AudioManager.Instance.Play("Yeehaw");

        while (_elapsedTime < _duration)
        {
            _elapsedTime += Time.deltaTime;
            float _newFontSize = Mathf.Lerp(initialFontSize, _targetFontSize, _elapsedTime / _duration);

            foreach (var _countDownText in countDownText)
            {
                _countDownText.fontSize = Mathf.RoundToInt(_newFontSize);
            }

            yield return null;
        }
        
        foreach (var _countDownText in countDownText)
        {
            _countDownText.fontSize = Mathf.RoundToInt(_targetFontSize);
        }
        
        //StartCoroutine(GetComponentInChildren<ShowDirectionOfWinArea>().RotateArrowToWinningArea());

        foreach (var _countDownText in countDownText)
        {
            _countDownText.text = "";
        }
    }
}
