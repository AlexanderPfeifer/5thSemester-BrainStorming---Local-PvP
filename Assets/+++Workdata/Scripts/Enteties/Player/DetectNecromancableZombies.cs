using System.Collections.Generic;
using UnityEngine;

public class DetectNecromancableZombies : MonoBehaviour
{
    [Header("Detect Necromance")]
    [SerializeField] private LayerMask humanLayer;
    [DisplayColor(0, 1, 0), SerializeField] private float detectNecromancableHordeRadius;
    private CachedZombieData cachedZombieData;
    private HashSet<Transform> necromancableHordeSet = new();
    private List<Transform> necromancableZombie = new();

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
            var _parent = _necromancableZombie.transform.parent;
            
            if(!necromancableZombie.Contains(_necromancableZombie.transform))
                necromancableZombie.Add(_necromancableZombie.transform);

            if (necromancableHordeSet.Contains(_parent))
                continue;

            necromancableHordeSet.Add(_parent);
        }
        
        // Create a temporary list to store parents because otherwise if the loop gets cancelled, it would result in an error
        var _removableHordeSet = new HashSet<Transform>();
        
        //Make the same for the hordeSets for the same reason as above
        var _tempHordeSet = new HashSet<Transform>(necromancableHordeSet);

        foreach (var _horde in _tempHordeSet)
        {
            bool _isZombieInRange  = false;
            
            foreach (Transform _zombie in necromancableZombie)
            {
                if (_zombie == null)
                {
                    break;
                }
 
                if (Vector3.Distance(_zombie.position, transform.position) < detectNecromancableHordeRadius)
                {
                    _isZombieInRange = true; 
                    break; 
                }
            }

            if (_isZombieInRange)
            {
                if (!cachedZombieData.NecromanceHorde.necromancableZombieHorde.Contains(_horde) && _horde != null)
                {
                    cachedZombieData.NecromanceHorde.necromancableZombieHorde.Add(_horde);
                    ShowNecromanceTextOnHordeGroups(_horde, 1);
                }
            }
            else
            {
                if (cachedZombieData.NecromanceHorde.necromancableZombieHorde.Contains(_horde) && _horde != null)
                {
                    cachedZombieData.NecromanceHorde.necromancableZombieHorde.Remove(_horde);
                    ShowNecromanceTextOnHordeGroups(_horde, 0); 
                }
            }
        }

        foreach (var _removableHorde in _removableHordeSet)
        {
            if (necromancableHordeSet.Contains(_removableHorde))
            {
                necromancableHordeSet.Remove(_removableHorde);
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectNecromancableHordeRadius);
    }
}
