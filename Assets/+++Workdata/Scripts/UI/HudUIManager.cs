using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class HudUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] countDownText;

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
        StartCoroutine(CountDownCoroutine());
    }

    private IEnumerator CountDownCoroutine()
    {
        foreach (var _countDownText in countDownText)
        {
            _countDownText.text = "3";
            yield return new WaitForSeconds(1);
        
            _countDownText.text = "2";
            yield return new WaitForSeconds(1);
        
            _countDownText.text = "1";
            yield return new WaitForSeconds(1);
        
            _countDownText.text = "GO!";
            yield return new WaitForSeconds(1);
        
            StartCoroutine(GetComponentInChildren<ShowDirectionOfWinArea>().RotateArrowToWinningArea());

            _countDownText.text = "";   
        }
    }
}
