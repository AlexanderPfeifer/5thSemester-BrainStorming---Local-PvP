using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lever : MonoBehaviour
{
    [SerializeField] private float maxLeverCooldown;
    [HideInInspector] public float currentLeverCooldown;
    
    public Image pullLeverImage;
    public CanvasGroup canvasGroup;

    private void Update()
    {
        if(currentLeverCooldown >= 0)
            currentLeverCooldown -= Time.deltaTime;
    }

    public void PullLever()
    {
        if (pullLeverImage.fillAmount >= 1)
        {
            AudioManager.Instance.Play("Necromance", true);
            
            List<WinningArea> _deactivatedWinningArea = new();

            foreach (var _winningArea in FindAnyObjectByType<ShowDirectionOfWinArea>().winningArea)
            {
                if (_winningArea.canObtainPoints)
                {
                    _winningArea.canObtainPoints = false;
                }
                else
                {
                    _deactivatedWinningArea.Add(_winningArea);
                }
            }

            if (Random.value > .5f)
            {
                _deactivatedWinningArea[0].canObtainPoints = true;
            }
            else
            {
                _deactivatedWinningArea[1].canObtainPoints = true;
            }

            currentLeverCooldown = maxLeverCooldown;   
        }
    }
}
