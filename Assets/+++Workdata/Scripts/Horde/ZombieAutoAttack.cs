using UnityEngine;

public class ZombieAutoAttack : MonoBehaviour
{
    [SerializeField] private float detectHumanRadius;
    [SerializeField] private float attackHumanRadius;
    [SerializeField] private float keepDistanceFromZombiesRadius;
    [SerializeField] private LayerMask humanLayer;
    [SerializeField] private LayerMask zombieLayer;
    [SerializeField] private HordeMovement hordeMovement;
    [SerializeField] private Sprite zombieSprite;
    private Transform closestHuman;
    private Transform closestZombie;
    private Collider2D[] humanHit;
    
    private void Update()
    {
        AutoAttack();

        KeepDistanceFromOtherZombies();
    }

    private void KeepDistanceFromOtherZombies()
    {
        Collider2D[] zombieHit = Physics2D.OverlapCircleAll(transform.position, keepDistanceFromZombiesRadius, zombieLayer);

        if (humanHit.Length == 0 && zombieHit.Length == 1 && zombieHit[0] == GetComponent<Collider2D>())
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.parent.position, 
                hordeMovement.CurrentHordeSpeed * Time.deltaTime);
            return;
        }

        foreach (Collider2D zombie in zombieHit)
        {
            if (closestZombie != null && (zombie.transform.position - transform.position).sqrMagnitude < (closestZombie.transform.position - transform.position).sqrMagnitude)
            {
                closestZombie = zombie.transform;
            }
            else
            {
                closestZombie = zombie.transform;
            }
        }

        closestZombie.transform.position = new Vector3(closestZombie.transform.position.x, closestZombie.transform.position.y, 0);
        
        transform.position = Vector3.MoveTowards(transform.position,  closestZombie.position, 
            -1 * hordeMovement.CurrentHordeSpeed * Time.deltaTime);
    }

    private void AutoAttack()
    {
        humanHit = Physics2D.OverlapCircleAll(transform.position, detectHumanRadius, humanLayer);

        if(humanHit.Length == 0)
            return;
            
        closestHuman = humanHit[0].transform;

        foreach (Collider2D human in humanHit)
        {
            if ((human.transform.position - transform.position).sqrMagnitude < (closestHuman.transform.position - transform.position).sqrMagnitude)
            {
                closestHuman = human.transform;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, closestHuman.position, 
            Time.deltaTime * hordeMovement.CurrentHordeSpeed);

        if ((transform.position - closestHuman.position).sqrMagnitude < 0.1f)
        {
            closestHuman.GetComponent<ZombieAutoAttack>().enabled = true;
            closestHuman.gameObject.layer = gameObject.layer;
            closestHuman.GetComponentInChildren<SpriteRenderer>().sprite = zombieSprite;
            closestHuman.SetParent(transform.parent);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectHumanRadius);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackHumanRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, keepDistanceFromZombiesRadius);
    }
}
