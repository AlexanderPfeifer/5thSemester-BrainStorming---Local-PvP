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
                    AutoAttack necromancableHordeAutoAttack = zombieHorde.GetChild(i).GetComponent<AutoAttack>();
                    necromancableHordeAutoAttack.attackableZombieLayer = GetComponent<AutoAttack>().attackableZombieLayer;
                    necromancableHordeAutoAttack.gameObject.layer = gameObject.layer;

                    ZombieMovement necromancableHordeZombieMovement = zombieHorde.GetChild(i).GetComponent<ZombieMovement>();
                    necromancableHordeZombieMovement.enabled = true;
                    necromancableHordeZombieMovement.ownZombieLayer = GetComponent<ZombieMovement>().ownZombieLayer;
                    //We can get the same zombie layer of this object because the zombie is going to join this zombies team
                    
                    SpriteRenderer necromancableHordeSr = zombieHorde.GetChild(i).GetComponentInChildren<SpriteRenderer>();
                    necromancableHordeSr.sprite = zombieSprite;
                    necromancableHordeSr.enabled = true;
                    
                    Health necromancableHordeHealth = zombieHorde.GetChild(i).GetComponent<Health>();
                    necromancableHordeHealth.isDead = false;
                    
                    Necromance necromancableHordeNecromance = zombieHorde.GetChild(i).GetComponent<Necromance>();
                    necromancableHordeNecromance.enabled = true;
                    
                    Animator necromancableHordeAnim = zombieHorde.GetChild(i).GetComponentInChildren<Animator>();
                    necromancableHordeAnim.enabled = true;
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
