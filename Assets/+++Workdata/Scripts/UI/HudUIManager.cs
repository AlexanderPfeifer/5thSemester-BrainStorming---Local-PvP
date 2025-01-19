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
        for (int _i = 3; _i > 0; _i--)
        {
            // Update all countDownText elements simultaneously
            foreach (var _countDownText in countDownText)
            {
                _countDownText.text = _i.ToString();
            }
            yield return new WaitForSeconds(1);
        }

        // Show "GO!" for all countDownText elements
        foreach (var _countDownText in countDownText)
        {
            _countDownText.text = "GO!";
        }
        yield return new WaitForSeconds(1);

        StartCoroutine(GetComponentInChildren<ShowDirectionOfWinArea>().RotateArrowToWinningArea());

        foreach (var _countDownText in countDownText)
        {
            _countDownText.text = "";
        }
    }
}
