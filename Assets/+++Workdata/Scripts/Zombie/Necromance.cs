using System.Collections.Generic;
using UnityEngine;

public class Necromance : MonoBehaviour
{
    [Header("Necromance")]
    [SerializeField] private Sprite zombieSprite;
    [DisplayColor(0, 1, 0), SerializeField] private float detectNecromancableHordeRadius;
    private List<Transform> necromancableZombieHorde = new List<Transform>();
    [SerializeField] private LayerMask graveLayer;
    [SerializeField] private GameObject necromanceText;
    private List<Transform> zombieWithWrongParentList;

    private void Update()
    {
        IdentifyNecromancableHorde();
    }
    
    public void OnNecromance()
    {
        if (necromancableZombieHorde.Count == 0)
            return;

        foreach (Transform zombieHorde in necromancableZombieHorde)
        {
            zombieWithWrongParentList = new List<Transform>();

            foreach (Transform zombie in zombieHorde.transform)
            {
                NecromanceZombie(zombie, gameObject);
            }

            foreach(var zombieWithWrongParent in zombieWithWrongParentList)
            {
                zombieWithWrongParent.parent = transform.parent;
            }
        }
    }

    public void NecromanceZombie(Transform necromancableZombie, GameObject zombieOfHorde)
    {
        var zombieOfHordeCachedData = zombieOfHorde.GetComponent<CachedZombieData>();
        var necromancableZombieCachedData = necromancableZombie.GetComponent<CachedZombieData>();

        necromancableZombieCachedData.AutoAttack.attackableZombieLayer = zombieOfHordeCachedData.AutoAttack.attackableZombieLayer;
        necromancableZombieCachedData.AutoAttack.gameObject.layer = zombieOfHorde.layer;
        necromancableZombieCachedData.AutoAttack.ResetAttack();



        //We can get the same zombie layer of this object because the zombie is going to join this zombies team
        necromancableZombieCachedData.ZombieMovement.TargetGroup = zombieOfHordeCachedData.ZombieMovement.TargetGroup;
        necromancableZombieCachedData.ZombieMovement.ZombiePlayerHordeRegistry = zombieOfHordeCachedData.ZombieMovement.ZombiePlayerHordeRegistry;
        necromancableZombieCachedData.ZombieMovement.enabled = true;



        necromancableZombieCachedData.SpriteRenderer.sprite = zombieOfHordeCachedData.Necromance.zombieSprite;
        necromancableZombieCachedData.SpriteRenderer.enabled = true;



        necromancableZombieCachedData.Health.ResetHealth();
        necromancableZombieCachedData.Health.isDead = false;
        necromancableZombieCachedData.Health.isPlayer = true;



        necromancableZombieCachedData.Necromance.enabled = true;
        necromancableZombieCachedData.Necromance.necromanceText.SetActive(false);



        necromancableZombieCachedData.Animator.enabled = true;



        zombieWithWrongParentList.Add(necromancableZombie);
        necromancableZombie.name = zombieOfHorde.name;
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

                if(_graveCount == graveHit.transform.parent.childCount && !necromancableZombieHorde.Contains(graveHit.transform.parent))
                {
                    necromancableZombieHorde.Add(graveHit.transform.parent);

                    foreach (var zombieHorde in necromancableZombieHorde)
                    {
                        foreach (Transform zombie in zombieHorde.transform)
                        {
                            zombie.GetComponent<Necromance>().necromanceText.SetActive(true);
                        }
                    }
                }
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D otherTrigger)
    {
        // Remove the horde from the list when it exits the range
        Transform hordeTransform = otherTrigger.transform;
        
        if (necromancableZombieHorde.Contains(hordeTransform))
        {
            foreach (Transform zombie in hordeTransform)
            {
                zombie.GetComponent<Necromance>().necromanceText.SetActive(false);
            }

            necromancableZombieHorde.Remove(hordeTransform);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectNecromancableHordeRadius);
    }
}
