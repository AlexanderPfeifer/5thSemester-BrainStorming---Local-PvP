using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class DetectInteractable : MonoBehaviour
{
    [Header("Detection")]
    [FormerlySerializedAs("detectNecromancableHordeRadius")] [DisplayColor(0, 1, 0), SerializeField] private float detectInteractableRadius;
    
    [Header("Necromance")]
    [SerializeField] private LayerMask humanLayer;
    private readonly HashSet<Transform> necromancableZombie = new();
    private readonly HashSet<Transform> necromancableHordeSet = new();
    [SerializeField] private LayerMask playerLayer;
    
    [Header("Brain")]
    [SerializeField] private LayerMask brainLayer;
    private Transform brain;
    
    [Header("Lever")]
    [SerializeField] private LayerMask leverLayer;
    private Transform lever;
    
    private CachedZombieData cachedZombieData;
    
    private void Start()
    {
        cachedZombieData = GetComponent<CachedZombieData>();
    }

    private void Update()
    {
        DetectNecromancableHorde();
        
        DetectLever();
        
        DetectBrain();
    }

    private void ShowInteractableImageOnZombies(Transform horde, float visibility)
    {
        horde.GetComponent<ShowNecromanceTriangle>().CanvasGroupVisibility(visibility);
    }

    private void ShowInteractableImageOnLever(Transform lever, float visibility)
    {
        lever.GetComponent<Lever>().canvasGroup.alpha = visibility;
    }
    
    private void ShowInteractableImageOnBrain(Transform brain, float visibility, bool resetFillAmout)
    {
        brain.GetComponent<WinningArea>().canvasGroup.alpha = visibility;

        if (resetFillAmout)
        {
            brain.GetComponent<WinningArea>().interactImage.fillAmount = 0;
        }
    }
    
    private void DetectBrain()
    {
        var _brainHit = Physics.OverlapSphere(transform.position, detectInteractableRadius, brainLayer);

        brain = _brainHit.Length > 0 ? _brainHit[0].transform : null;
        
        if(brain == null)
            return;
        
        var _playerZombies = Physics.OverlapSphere(transform.position, brain.GetComponent<WinningArea>().zombiesInRangeRadius, playerLayer);

        if (_playerZombies.Length > 1 && brain.GetComponent<WinningArea>().canObtainPoints 
                                      && Vector3.Distance(transform.position, brain.position) < detectInteractableRadius)
        {
            ShowInteractableImageOnBrain(brain, 1, false);
            cachedZombieData.NecromanceHorde.InteractableBrain = _brainHit[0].transform;
            
            foreach (var _player1Zombie in _playerZombies)
            {
                if (!cachedZombieData.NecromanceHorde.zombiesNearBrainPlayer.Contains(_player1Zombie))
                    cachedZombieData.NecromanceHorde.zombiesNearBrainPlayer.Add(_player1Zombie);
            }
        }
        else if ((playerLayer.value & (1 << gameObject.layer)) != 0 
                 && _playerZombies.Length == 1 
                 && brain.GetComponent<WinningArea>().canObtainPoints 
                 && Vector3.Distance(transform.position, brain.position) < detectInteractableRadius)
        {
            if (brain.GetComponent<WinningArea>().canObtainPoints)
            {
                GetComponentInChildren<CanvasGroup>().alpha = 1;
            }
        }
        else
        {
            GetComponentInChildren<CanvasGroup>().alpha = 0;
            ShowInteractableImageOnBrain(brain, 0, true);
            cachedZombieData.NecromanceHorde.InteractableBrain = null; 
            cachedZombieData.NecromanceHorde.zombiesNearBrainPlayer.Clear();
            cachedZombieData.NecromanceHorde.zombiesNearBrainPlayer.Clear();
        }
    }

    private void DetectLever()
    {
        var _leverHit = Physics.OverlapSphere(transform.position, detectInteractableRadius, leverLayer);

        lever = _leverHit.Length > 0 ? _leverHit[0].transform : null;

        if(lever == null)
            return;
        
        if (Vector3.Distance(transform.position, lever.position) < detectInteractableRadius)
        {
            ShowInteractableImageOnLever(lever, 1);
            cachedZombieData.NecromanceHorde.InteractableLever = _leverHit[0].transform;
        }
        else
        {
            ShowInteractableImageOnLever(lever, 0);
            cachedZombieData.NecromanceHorde.InteractableLever = null;
            lever = null;
        }
    }

    private void DetectNecromancableHorde()
    {
        var _necromancableZombieHit = Physics.OverlapSphere(transform.position, detectInteractableRadius, humanLayer);
        
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
            bool _isZombieInRange = necromancableZombie.Any(zombie =>
                zombie != null && Vector3.Distance(zombie.position, transform.position) < detectInteractableRadius);

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
        necromancableHordeSet.RemoveWhere(horde => !_tempHordeSet.Contains(horde));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectInteractableRadius);
    }
}
