using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class HudUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countDownText;

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
        countDownText.text = "3";
        yield return new WaitForSeconds(1);
        
        countDownText.text = "2";
        yield return new WaitForSeconds(1);
        
        countDownText.text = "1";
        yield return new WaitForSeconds(1);
        
        countDownText.text = "GO!";
        yield return new WaitForSeconds(1);

        countDownText.text = "";
    }
}
