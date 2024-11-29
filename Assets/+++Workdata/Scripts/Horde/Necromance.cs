using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Necromance : MonoBehaviour
{
    [Header("Necromance")]
    [SerializeField] private Sprite zombieSprite;
    [SerializeField] private float detectNecromancableHordeRadius;
    private List<Transform> necromancableZombieHorde = new List<Transform>();
    [SerializeField] public LayerMask graveLayer;

    private void Update()
    {
        IdentifyNecromancableHorde();
        
        NecromanceZombieGroup();
    }
    
    private void NecromanceZombieGroup()
    {
        if (necromancableZombieHorde.Count > 0 && Input.GetKeyDown(KeyCode.E))
        {
            foreach (var zombieHorde in necromancableZombieHorde)
            {
                for (int i = 0; i < zombieHorde.childCount; i++)
                {
                    AutoAttack hordeChildAutoAttack = zombieHorde.GetChild(i).GetComponent<AutoAttack>();
                    hordeChildAutoAttack.attackableZombieLayer = GetComponent<AutoAttack>().attackableZombieLayer;
                    hordeChildAutoAttack.gameObject.layer = gameObject.layer;

                    ZombieMovement hordeChildZombieMovement = zombieHorde.GetChild(i).GetComponent<ZombieMovement>();
                    hordeChildZombieMovement.enabled = true;
                    //hordeChildZombieMovement.ownZombieLayer = GetComponent<ZombieMovement>().ownZombieLayer;
                    
                    SpriteRenderer hordeChildSr = zombieHorde.GetChild(i).GetComponentInChildren<SpriteRenderer>();
                    hordeChildSr.sprite = zombieSprite;
                    hordeChildSr.enabled = true;
                    
                    zombieHorde.GetChild(i).GetComponentInChildren<Animator>().enabled = true;
                }
            }
        }
    }
    
    private void IdentifyNecromancableHorde()
    {
        var necromancableHordeHit = Physics2D.OverlapCircleAll(transform.position, detectNecromancableHordeRadius, graveLayer);

        foreach (var graveHit in necromancableHordeHit)
        {
            for (int i = 0; i < graveHit.transform.parent.childCount; i++)
            {
                if (!graveHit.transform.parent.GetChild(i).GetComponent<Health>().isDead)
                {
                    break;
                }

                necromancableZombieHorde.Add(graveHit.transform.parent);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D otherTrigger)
    {
        // Remove the horde from the list when it exits the range
        Transform hordeTransform = otherTrigger.transform;
        
        if (necromancableZombieHorde.Contains(hordeTransform))
        {
            necromancableZombieHorde.Remove(hordeTransform);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectNecromancableHordeRadius);
    }
}
