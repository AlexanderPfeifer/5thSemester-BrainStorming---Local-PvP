using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DetectNecromancableZombies : MonoBehaviour
{
    [Header("Detect Necromance")]
    [SerializeField] private LayerMask humanLayer;
    [DisplayColor(0, 1, 0), SerializeField] private float detectNecromancableHordeRadius;
    private CachedZombieData cachedZombieData;
    
    private readonly HashSet<Transform> necromancableHordeSet = new();
    private readonly HashSet<Transform> necromancableZombie = new();

    private void Start()
    {
        cachedZombieData = GetComponent<CachedZombieData>();
    }

    private void Update()
    {
        IdentifyNecromancableHorde();
    }
    
    private void ShowNecromanceTextOnHordeGroups(Transform horde, float visibility)
    {
        horde.GetComponent<ShowNecromanceText>().CanvasGroupVisibility(visibility);
    }

    private void IdentifyNecromancableHorde()
    {
        var _necromancableZombieHit = Physics.OverlapSphere(transform.position, detectNecromancableHordeRadius, humanLayer);

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
                _zombie != null && Vector3.Distance(_zombie.position, transform.position) < detectNecromancableHordeRadius);

            // Activate or deactivate necromance text based on zombies in range
            if (_isZombieInRange)
            {
                if (_horde != null && cachedZombieData.NecromanceHorde.NecromancableZombieHorde.Add(_horde))
                {
                    ShowNecromanceTextOnHordeGroups(_horde, 1);
                }
            }
            else
            {
                if (_horde != null && cachedZombieData.NecromanceHorde.NecromancableZombieHorde.Remove(_horde)) // Only remove if present
                {
                    ShowNecromanceTextOnHordeGroups(_horde, 0);
                }
            }
        }
        
        necromancableZombie.Clear();
        
        // Remove hordes marked as removable
        necromancableHordeSet.RemoveWhere(_horde => !_tempHordeSet.Contains(_horde));
    }

    void RemoveNecromancableZombies()
    {
        
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectNecromancableHordeRadius);
    }
}
