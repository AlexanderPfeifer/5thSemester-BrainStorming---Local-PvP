using System.Collections.Generic;
using UnityEngine;

public class DetectNecromancableZombies : MonoBehaviour
{
    [Header("Detect Necromance")]
    [SerializeField] private LayerMask graveLayer;
    [DisplayColor(0, 1, 0), SerializeField] private float detectNecromancableHordeRadius;
    private CachedZombieData cachedZombieData;

    private void Start()
    {
        cachedZombieData = GetComponent<CachedZombieData>();
    }

    private void Update()
    {
        IdentifyNecromancableHorde();
    }

    private void IdentifyNecromancableHorde()
    {
        var necromancableZombieHit = Physics2D.OverlapCircleAll(transform.position, detectNecromancableHordeRadius, graveLayer);

        // Create a temporary list to store parents to add to the horde list
        var necromancableHordeSet = new HashSet<Transform>(cachedZombieData.NecromanceHorde.necromancableZombieHorde);

        foreach (var necromancableZombie in necromancableZombieHit)
        {
            var parent = necromancableZombie.transform.parent;

            // Check if the parent is already in the horde set
            if (necromancableHordeSet.Contains(parent))
                continue;

            bool allDead = true;
            foreach (Transform zombie in parent)
            {
                if (!zombie.GetComponent<Health>().isDead)
                {
                    allDead = false;
                    break;
                }
            }

            if (allDead)
            {
                foreach (Transform zombie in parent)
                {
                    zombie.GetComponent<ShowNecromanceText>().wholeHordeDead = true;
                }

                necromancableHordeSet.Add(parent);
            }
        }

        // Add all new hordes to the main list after the loop is complete
        foreach (var horde in necromancableHordeSet)
        {
            cachedZombieData.NecromanceHorde.necromancableZombieHorde.Add(horde);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectNecromancableHordeRadius);
    }
}
