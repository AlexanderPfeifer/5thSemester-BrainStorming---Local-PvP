using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class DetectInteractable : MonoBehaviour
{
    [Header("Detect Necromance")]
    [SerializeField] private LayerMask humanLayer;
    [SerializeField] private LayerMask leverLayer;
    [FormerlySerializedAs("detectNecromancableHordeRadius")] [DisplayColor(0, 1, 0), SerializeField] private float detectInteractableRadius;
    private CachedZombieData cachedZombieData;
    
    private readonly HashSet<Transform> necromancableHordeSet = new();
    private readonly HashSet<Transform> necromancableZombie = new();

    [SerializeField] private ParticleSystem interactableInRange;
    
    private Transform lever;

    private bool leverDetected;
    private bool humanDetected;

    private void Start()
    {
        cachedZombieData = GetComponent<CachedZombieData>();
    }

    private void Update()
    {
        DetectNecromancableHorde();
        
        DetectLever();
        
        ShowInteractionParticles();
    }

    private void ShowInteractionParticles()
    {
        if (leverDetected && humanDetected)
        {
            if(!interactableInRange.isPlaying)
                interactableInRange.Play();   
        }
        else
        {
            interactableInRange.Stop();
        }
    }
    
    private void ShowInteractableImageOnZombies(Transform horde, float visibility)
    {
        horde.GetComponent<ShowNecromanceText>().CanvasGroupVisibility(visibility);
    }

    private void ShowInteractableImageOnLever(Transform lever, float visibility, bool resetFillAmout)
    {
        lever.GetComponent<Lever>().canvasGroup.alpha = visibility;

        if (resetFillAmout)
        {
            lever.GetComponent<Lever>().pullLeverImage.fillAmount = 0;
        }
    }

    private void DetectLever()
    {
        var _leverHit = Physics.OverlapSphere(transform.position, detectInteractableRadius, leverLayer);

        if (_leverHit.Length > 0)
        {
            leverDetected = true;            
            lever = _leverHit[0].transform;
        }
        else
        {
            leverDetected = false;
        }
        
        if(lever == null)
            return;
        
        if (Vector3.Distance(transform.position, lever.position) < detectInteractableRadius && _leverHit[0].GetComponent<Lever>().currentLeverCooldown <= 0)
        {
            ShowInteractableImageOnLever(lever, 1, false);
            cachedZombieData.NecromanceHorde.InteractableLever = _leverHit[0].transform;
        }
        else
        {
            ShowInteractableImageOnLever(lever, 0, true);
            cachedZombieData.NecromanceHorde.InteractableLever = null; 
        }
    }

    private void DetectNecromancableHorde()
    {
        var _necromancableZombieHit = Physics.OverlapSphere(transform.position, detectInteractableRadius, humanLayer);
        humanDetected = _necromancableZombieHit.Length > 0;
        
        foreach (var _necromancableZombie in _necromancableZombieHit)
        {
            var _zombieTransform = _necromancableZombie.transform;
            var _zombieParent = _zombieTransform.parent;

            if(necromancableZombie.Add(_zombieTransform))
            {}
            
            if (_zombieParent != null && necromancableHordeSet.Add(_zombieParent))  // Add returns false if the item already exists
            {}
        }
        
        // Temporary hash set for safe iteration
        var _tempHordeSet = new HashSet<Transform>(necromancableHordeSet);

        foreach (var _horde in _tempHordeSet)
        {
            bool _isZombieInRange = necromancableZombie.Any(_zombie =>
                _zombie != null && Vector3.Distance(_zombie.position, transform.position) < detectInteractableRadius);

            // Activate or deactivate necromance text based on zombies in range
            if (_isZombieInRange)
            {
                if (_horde != null && cachedZombieData.NecromanceHorde.NecromancableZombieHorde.Add(_horde))
                {
                    ShowInteractableImageOnZombies(_horde, 1);
                }
            }
            else
            {
                if (_horde != null && cachedZombieData.NecromanceHorde.NecromancableZombieHorde.Remove(_horde)) // Only remove if present
                {
                    ShowInteractableImageOnZombies(_horde, 0);
                }
            }
        }
        
        necromancableZombie.Clear();
        
        // Remove hordes marked as removable
        necromancableHordeSet.RemoveWhere(_horde => !_tempHordeSet.Contains(_horde));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectInteractableRadius);
    }
}
