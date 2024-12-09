using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Necromance : MonoBehaviour
{
    [Header("Necromance")]
    [SerializeField] private Sprite zombieSprite;
    [DisplayColorAbove(0, 1, 0), SerializeField] private float detectNecromancableHordeRadius;
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
                foreach (Transform zombie in zombieHorde.transform)
                {
                    AutoAttack necromancableHordeAutoAttack = zombie.GetComponent<AutoAttack>();
                    necromancableHordeAutoAttack.attackableZombieLayer = GetComponent<AutoAttack>().attackableZombieLayer;
                    necromancableHordeAutoAttack.gameObject.layer = gameObject.layer;

                    ZombieMovement necromancableHordeZombieMovement = zombie.GetComponent<ZombieMovement>();
                    //We can get the same zombie layer of this object because the zombie is going to join this zombies team
                    necromancableHordeZombieMovement.ownZombieLayer = GetComponent<ZombieMovement>().ownZombieLayer;
                    necromancableHordeZombieMovement.targetGroup = GetComponent<ZombieMovement>().targetGroup;
                    necromancableHordeZombieMovement.zombieManager = GetComponent<ZombieMovement>().zombieManager;

                    necromancableHordeZombieMovement.enabled = true;

                    SpriteRenderer necromancableHordeSr = zombie.GetComponentInChildren<SpriteRenderer>();
                    necromancableHordeSr.sprite = zombieSprite;
                    necromancableHordeSr.enabled = true;

                    Health necromancableHordeHealth = zombie.GetComponent<Health>();
                    necromancableHordeHealth.isDead = false;
                    necromancableHordeHealth.isPlayer = true;

                    Necromance necromancableHordeNecromance = zombie.GetComponent<Necromance>();
                    necromancableHordeNecromance.enabled = true;

                    Animator necromancableHordeAnim = zombie.GetComponentInChildren<Animator>();
                    necromancableHordeAnim.enabled = true;

                    zombie.parent = transform.parent;
                    zombie.name = transform.name;
                }

                //Destroy the parent object because we don't need it anymore and with a one second delay because otherwise some zombies malfunction
                Destroy(zombieHorde.gameObject, 1);
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
