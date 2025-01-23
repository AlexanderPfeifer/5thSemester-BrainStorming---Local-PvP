using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class DetectInteractable : MonoBehaviour
{
    [Header("Detect Necromance")]
    [SerializeField] private LayerMask humanLayer;
    [SerializeField] private LayerMask leverLayer;
    [SerializeField] private LayerMask brainLayer;
    [SerializeField] private LayerMask playerLayer;
    [FormerlySerializedAs("detectNecromancableHordeRadius")] [DisplayColor(0, 1, 0), SerializeField] private float detectInteractableRadius;
    private CachedZombieData cachedZombieData;
    
    private readonly HashSet<Transform> necromancableHordeSet = new();
    private readonly HashSet<Transform> necromancableZombie = new();
    
    private Transform lever;
    private Transform brain;
    
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
        horde.GetComponent<ShowNecromanceText>().CanvasGroupVisibility(visibility);
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
            brain.GetComponent<WinningArea>().obtainPointsImage.fillAmount = 0;
        }
    }
    
    private void DetectBrain()
    {
        var _brainHit = Physics.OverlapSphere(transform.position, detectInteractableRadius, brainLayer);

        if (_brainHit.Length > 0)
        {
            brain = _brainHit[0].transform;
        }
        else
        {
            brain = null;
        }
        
        if(brain == null)
            return;
        
        var _playerZombies = Physics.OverlapSphere(transform.position, brain.GetComponent<WinningArea>().zombiesInRangeRadius, playerLayer);

        if (_playerZombies.Length > 1)
        {
            if (Vector3.Distance(transform.position, brain.position) < detectInteractableRadius && 
                brain.GetComponent<WinningArea>().canObtainPoints)
            {
                ShowInteractableImageOnBrain(brain, 1, false);
                cachedZombieData.NecromanceHorde.InteractableBrain = _brainHit[0].transform;
                foreach (var _player1Zombie in _playerZombies)
                {
                    if (!cachedZombieData.NecromanceHorde.zombiesNearBrainPlayer.Contains(_player1Zombie))
                        cachedZombieData.NecromanceHorde.zombiesNearBrainPlayer.Add(_player1Zombie);
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
            
            if ((playerLayer.value & (1 << gameObject.layer)) != 0 && _playerZombies.Length == 1 && brain.GetComponent<WinningArea>().canObtainPoints)
            {
                if (brain.GetComponent<WinningArea>().canObtainPoints)
                {
                    GetComponentInChildren<CanvasGroup>().alpha = 1;
                }
            }
            else
            {
                GetComponentInChildren<CanvasGroup>().alpha = 0;
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

        if (_leverHit.Length > 0)
        {
            lever = _leverHit[0].transform;
        }
        else
        {
            lever = null;
        }

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
