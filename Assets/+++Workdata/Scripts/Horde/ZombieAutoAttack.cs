using UnityEngine;
using UnityEngine.Serialization;

public class ZombieAutoAttack : MonoBehaviour
{
    [SerializeField] private float detectHumanRadius;
    [SerializeField] private float attackHumanRadius;
    [SerializeField] private float keepDistanceFromZombiesRadius;
    [SerializeField] private float closeEnoughToHordeRadius;
    [FormerlySerializedAs("zombieOtherTeam")] [FormerlySerializedAs("humanLayer")] [SerializeField] private LayerMask zombieOtherTeamLayer;
    [SerializeField] private LayerMask zombieLayer;
    [SerializeField] private HordeMovement hordeMovement;

    [SerializeField] private int damage;
    
    private Transform closestZombieOtherTeam;
    private Transform closestZombieOwnTeam;
    private Collider2D[] zombieHitOtherTeam;

    private Animator animator;
    
    private float maxTimeUntilNextAttack;
    private float currentTimeUntilNextAttack;
    
    private void Start()
    {
        hordeMovement = GetComponentInParent<HordeMovement>();

        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        AutoAttack();

        KeepDistanceFromOtherZombies();
    }

    private void KeepDistanceFromOtherZombies()
    {
        Collider2D[] zombieHitTooNear = Physics2D.OverlapCircleAll(transform.position, keepDistanceFromZombiesRadius, zombieLayer);
        Collider2D[] zombieHitCloseToHorde = Physics2D.OverlapCircleAll(transform.position, closeEnoughToHordeRadius, zombieLayer);

        //Asks if zombieHitCloseToHorde is smaller than 2 because the first hit is always itself
        if (zombieHitCloseToHorde.Length < 2 && zombieHitOtherTeam.Length == 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.parent.position, 
                hordeMovement.CurrentHordeSpeed * Time.deltaTime);
        }
        else if(zombieHitTooNear.Length > 1)
        {
            foreach (Collider2D zombie in zombieHitTooNear)
            {
                if (closestZombieOwnTeam != null && (zombie.transform.position - transform.position).sqrMagnitude < (closestZombieOwnTeam.transform.position - transform.position).sqrMagnitude)
                {
                    closestZombieOwnTeam = zombie.transform;
                }
                else
                {
                    closestZombieOwnTeam = zombie.transform;
                }
            }

            closestZombieOwnTeam.transform.position = new Vector3(closestZombieOwnTeam.transform.position.x, closestZombieOwnTeam.transform.position.y, 0);
        
            //Moves away from the closest zombie
            transform.position = Vector3.MoveTowards(transform.position,  closestZombieOwnTeam.position, 
                -1 * hordeMovement.CurrentHordeSpeed * Time.deltaTime);
        }
    }

    private void AutoAttack()
    {
        zombieHitOtherTeam = Physics2D.OverlapCircleAll(transform.position, detectHumanRadius, zombieOtherTeamLayer);
        
        Debug.Log(zombieHitOtherTeam.Length);

        if (zombieHitOtherTeam.Length == 0)
        {
            hordeMovement.StopMovement = false;
            return;   
        }

        closestZombieOtherTeam = zombieHitOtherTeam[0].transform;

        foreach (Collider2D human in zombieHitOtherTeam)
        {
            if ((human.transform.position - transform.position).sqrMagnitude < (closestZombieOtherTeam.transform.position - transform.position).sqrMagnitude)
            {
                closestZombieOtherTeam = human.transform;
            }
        }

        hordeMovement.StopMovement = true;

        transform.position = Vector3.MoveTowards(transform.position, closestZombieOtherTeam.position, 
            Time.deltaTime * hordeMovement.CurrentHordeSpeed);

        if ((transform.position - closestZombieOtherTeam.position).sqrMagnitude < 0.1f)
        {
            currentTimeUntilNextAttack -= Time.deltaTime;

            if (currentTimeUntilNextAttack < 0)
            {
                animator.SetTrigger("attack");
                currentTimeUntilNextAttack = maxTimeUntilNextAttack;
            }
        }
    }

    public void AttackEnemyAnimationEvent()
    {
        if(closestZombieOtherTeam == null)
            return;
        
        
        if (closestZombieOtherTeam.gameObject.layer == gameObject.layer)
        {
            closestZombieOtherTeam = null;
        }
        else
        {
            closestZombieOtherTeam.GetComponent<Health>().DamageIncome(gameObject, damage);
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
        
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, closeEnoughToHordeRadius);
    }
}
