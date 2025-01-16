using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lever : MonoBehaviour
{
    [SerializeField] private float maxLeverCooldown;
    [HideInInspector] public float currentLeverCooldown;
    
    public Image pullLeverImage;
    public CanvasGroup canvasGroup;

    [SerializeField] private ParticleSystem interactionParticles;

    [SerializeField] private Transform lever;
 
    private void Update()
    {
        if(currentLeverCooldown >= 0)
            currentLeverCooldown -= Time.deltaTime;
    }

    public void PullLever()
    {
        if (pullLeverImage.fillAmount >= 1 && currentLeverCooldown <= 0)
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
            
            interactionParticles.Play();
            
            if (Random.value > .5f)
            {
                _deactivatedWinningArea[0].canObtainPoints = true;
            }
            else
            {
                _deactivatedWinningArea[1].canObtainPoints = true;
            }
            
            StartCoroutine(PullLeverMotion());
            
            currentLeverCooldown = maxLeverCooldown;   
        }
    }

    private IEnumerator PullLeverMotion()
    {
        Quaternion _startRotation = lever.localRotation;                     // Current rotation
        Quaternion _targetRotation = Quaternion.Euler(-lever.localRotation.eulerAngles.x, 
            lever.localRotation.eulerAngles.y, 
            lever.localRotation.eulerAngles.z); // Desired rotation
        const float duration = 1f;                                          // Time for the motion
        float elapsedTime = 0f;                                             // Track elapsed time

        while (elapsedTime < duration)
        {
            // Interpolate between the start and target rotation using Quaternion.Lerp
            lever.localRotation = Quaternion.Lerp(_startRotation, _targetRotation, elapsedTime / duration);
        
            elapsedTime += Time.deltaTime; // Increment elapsed time
            yield return null;            // Wait until the next frame
        }

        // Ensure it ends exactly at the target
        lever.localRotation = _targetRotation;
    }
}
