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
        if (currentLeverCooldown >= 0)
        {
            pullLeverImage.fillAmount = currentLeverCooldown / maxLeverCooldown;

            currentLeverCooldown -= Time.deltaTime;
        }
    }

    public void PullLever()
    {
        if (pullLeverImage.fillAmount >= 1 && currentLeverCooldown <= 0)
        {
            AudioManager.Instance.Play("LeverCrank");
            
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
            
            StartCoroutine(PullLeverMotionCoroutine());
            
            currentLeverCooldown = maxLeverCooldown;
        }
    }

    private IEnumerator PullLeverMotionCoroutine()
    {
        var _rotation = lever.rotation;
        Vector3 _localEulerAngles = _rotation.eulerAngles;

        // Define the start and target rotations using Euler angles
        Quaternion _startRotation = _rotation;
        Quaternion _targetRotation = Quaternion.Euler(-_localEulerAngles.x, _localEulerAngles.y, _localEulerAngles.z);

        
        const float duration = 1f;                                         
        float _elapsedTime = 0f;                                             

        while (_elapsedTime < duration)
        {
            lever.rotation = Quaternion.Lerp(_startRotation, _targetRotation, _elapsedTime / duration);
        
            _elapsedTime += Time.deltaTime; 
            yield return null;          
        }

        lever.rotation = _targetRotation;
    }
}
