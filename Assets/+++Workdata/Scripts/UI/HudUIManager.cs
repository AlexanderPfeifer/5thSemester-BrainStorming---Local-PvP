using System.Collections;
using TMPro;
using UnityEngine;

public class HudUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] countDownText;
    [SerializeField] private GameObject[] arrows;
    private float initialFontSize;

    private void Start()
    {
        AudioManager.Instance.FadeIn("InGameMusic");
    }

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
        
        _elapsedTime = 0f;

        var _timeBetweenText = .5f;
        
        //Changed the font size here because the text is too big and would look weird when scaling
        foreach (var _countDownText in countDownText)
        {
            _countDownText.text = "BRING";
            _countDownText.fontSize = 100;
            _targetFontSize = 140f;
            initialFontSize = 100f;
        }
        
        yield return new WaitForSeconds(_timeBetweenText);

        foreach (var _countDownText in countDownText)
        {
            _countDownText.text = "BRING \n ZOMBIES";
            _countDownText.fontSize = 100;
            _targetFontSize = 140f;
            initialFontSize = 100f;
        }
        
        yield return new WaitForSeconds(_timeBetweenText);

        foreach (var _countDownText in countDownText)
        {
            _countDownText.text = "BRING \n ZOMBIES \n TO THE";
            _countDownText.fontSize = 100;
            _targetFontSize = 140f;
            initialFontSize = 100f;
        }
        
        yield return new WaitForSeconds(_timeBetweenText);
        
        foreach (var _countDownText in countDownText)
        {
            _countDownText.text = "BRING \n ZOMBIES \n TO THE \n BRAIN!";
            _countDownText.fontSize = 100;
            _targetFontSize = 140f;
            initialFontSize = 100f;
        }
        
        foreach (var _arrow in arrows)
        {
            _arrow.SetActive(true);
        }

        StartCoroutine(BlinkingArrows());
        
        FindAnyObjectByType<NPCSpawner>().OnSpawnSmallHordesOverTime();

        FindAnyObjectByType<Lever>().pullLeverImage.fillAmount = 1;
        FindAnyObjectByType<Lever>().PullLever(true);

        foreach (var _playerMovement in FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None))
        {
            _playerMovement.AllowMovement();
        }

        AudioManager.Instance.Play("Yeehaw");

        while (_elapsedTime < _duration)
        {
            _elapsedTime += Time.deltaTime;
            float _newFontSize = Mathf.Lerp(initialFontSize, _targetFontSize, _elapsedTime / _duration);
            
            float _newAlpha = Mathf.Lerp(1, 0, _elapsedTime / _duration);

            foreach (var _countDownText in countDownText)
            {
                _countDownText.fontSize = Mathf.RoundToInt(_newFontSize);
                _countDownText.alpha = _newAlpha;
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

    private IEnumerator BlinkingArrows()
    {
        yield return new WaitForSeconds(.5f);
        
        foreach (var _arrow in arrows)
        {
            _arrow.SetActive(false);
        }

        yield return new WaitForSeconds(.5f);
        
        foreach (var _arrow in arrows)
        {
            _arrow.SetActive(true);
        }
        
        yield return new WaitForSeconds(.5f);
        
        foreach (var _arrow in arrows)
        {
            _arrow.SetActive(false);
        }
        
        yield return new WaitForSeconds(.5f);
        
        foreach (var _arrow in arrows)
        {
            _arrow.SetActive(true);
        }
        
        yield return new WaitForSeconds(.5f);
        
        foreach (var _arrow in arrows)
        {
            _arrow.SetActive(false);
        }
    }
}
