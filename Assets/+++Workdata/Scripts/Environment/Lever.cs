using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Lever : MonoBehaviour
{
    [Header("Cooldown Time")]
    [SerializeField] private float maxLeverCooldown;
    [HideInInspector] public float currentLeverCooldown;
    
    [Header("UI")]
    [FormerlySerializedAs("pullLeverImage")] public Image interactionImage;
    public CanvasGroup canvasGroup;

    [Header("VFX")]
    [SerializeField] private ParticleSystem interactionParticles;

    [Header("Lever")]
    [SerializeField] private Transform lever;
 
    private void Update()
    {
        CooldownTime();
    }

    public void PullLever(bool changeCooldown)
    {
        if (interactionImage.fillAmount >= 1 && currentLeverCooldown <= 0)
        {
            AudioManager.Instance.Play("LeverCrank");
            
            List<WinningArea> _deactivatedWinningArea = new();
            
            foreach (var _winningArea in FindAnyObjectByType<WinningAreas>().winningArea)
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
            
            //keep cooldown is only used at the start so the game does not start with the lever having a cooldown
            if (changeCooldown)
                currentLeverCooldown = maxLeverCooldown;
        }
    }

    private IEnumerator PullLeverMotionCoroutine()
    {
        var _rotation = lever.rotation;
        Vector3 _localEulerAngles = _rotation.eulerAngles;

        Quaternion _startRotation = _rotation;
        //The target rotation is just always getting the minus X transform because it rotates back and forth always when pulled
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
    
    private void CooldownTime()
    {
        if (currentLeverCooldown >= 0)
        {
            interactionImage.fillAmount = currentLeverCooldown / maxLeverCooldown;

            currentLeverCooldown -= Time.deltaTime;
        }
    }
}
