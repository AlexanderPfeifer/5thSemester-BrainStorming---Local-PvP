using System.Collections.Generic;
using TMPro;
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

        ShowNecromanceText();
    }

    private void IdentifyNecromancableHorde()
    {
        var necromancableHordeHit = Physics2D.OverlapCircleAll(transform.position, detectNecromancableHordeRadius, graveLayer);

        foreach (var graveHit in necromancableHordeHit)
        {
            int _graveCount = 0;

            for (int i = 0; i < graveHit.transform.parent.childCount; i++)
            {
                if (!graveHit.transform.parent.GetChild(i).GetComponent<Health>().isDead)
                {
                    _graveCount = 0;
                    break;
                }

                _graveCount++;

                if(_graveCount == graveHit.transform.parent.childCount && !cachedZombieData.NecromanceHorde.necromancableZombieHorde.Contains(graveHit.transform.parent))
                {
                    cachedZombieData.NecromanceHorde.necromancableZombieHorde.Add(graveHit.transform.parent);
                }
            }
        }
    }

    private void ShowNecromanceText()
    {
        foreach (var zombieHorde in cachedZombieData.NecromanceHorde.necromancableZombieHorde)
        {
            foreach (Transform zombie in zombieHorde.transform)
            {
                zombie.GetComponent<Health>().NecromanceText.SetActive(true);
            }
        }
    }
    

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectNecromancableHordeRadius);
    }
}
