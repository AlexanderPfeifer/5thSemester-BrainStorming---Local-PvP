using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    [Header("Speed")] 
    [SerializeField] private float moveToEnemySpeed;
    
    [Header("Attack")]
    [SerializeField] public LayerMask attackableZombieLayer;
    [DisplayColor(1, 0, 0), SerializeField] private float attackRadius;
    [SerializeField] private int damage;
    [DisplayColor(1, 1, 0), SerializeField] private float detectEnemyZombiesRadius;
    [SerializeField] private float maxTimeUntilNextAttack;
    [HideInInspector] public bool isAttacking;
    private float currentTimeUntilNextAttack;
    private Transform closestAttackableZombie;

    private CachedZombieData cachedZombieData;

    private void Start()
    {
        cachedZombieData = GetComponent<CachedZombieData>();
    }

    private void Update()
    {
        if(GetComponent<Health>().isDead)
            return;

        IdentifyAttackableZombie();
    }

    private void IdentifyAttackableZombie()
    {
        Collider[] attackableZombieHit = Physics.OverlapSphere(transform.position, detectEnemyZombiesRadius, attackableZombieLayer);

        if (attackableZombieHit.Length > 0)
        {
            if (!isAttacking)
            {
                // Start with the first zombie
                closestAttackableZombie = attackableZombieHit[0].transform;

                foreach (Collider zombie in attackableZombieHit)
                {
                    // Compare squared distances to avoid unnecessary square root calculations
                    if ((zombie.transform.position - transform.position).sqrMagnitude < (closestAttackableZombie.position - transform.position).sqrMagnitude)
                    {
                        closestAttackableZombie = zombie.transform;
                    }
                }
            }

            HandleAttackBehaviour();
        }
        else
        {
            isAttacking = false;
        }
    }
    
    private void HandleAttackBehaviour()
    {
        if (closestAttackableZombie == null || closestAttackableZombie.GetComponent<Health>().isDead)
            return;

        if (Vector3.Distance(transform.position, closestAttackableZombie.position) < attackRadius)
        {
            currentTimeUntilNextAttack -= Time.deltaTime;

            if (currentTimeUntilNextAttack <= 0)
            {
                cachedZombieData.Animator.SetTrigger("attack");
                currentTimeUntilNextAttack = maxTimeUntilNextAttack;
            }
        }
        else
        {
            MoveTowardsClosestEnemy();
        }
    }

    private void MoveTowardsClosestEnemy()
    {
        Vector3 directionToEnemy = closestAttackableZombie.position - transform.position;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToEnemy.normalized, out hit, directionToEnemy.magnitude, 1 << gameObject.layer))
        {
            if (hit.collider.gameObject != gameObject || closestAttackableZombie.GetComponent<Health>().isDead)
            {
                return;
            }
        }

        isAttacking = true;

        transform.position = Vector3.MoveTowards(transform.position, closestAttackableZombie.position, Time.deltaTime * moveToEnemySpeed);
    }

    public void AttackEnemyAnimationEvent()
    {
        if (closestAttackableZombie != null && !closestAttackableZombie.GetComponent<Health>().isDead)
        {
            closestAttackableZombie.GetComponent<Health>().DamageIncome(damage, this);
        }
    }

    public void ResetAttack()
    {
        closestAttackableZombie = null;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectEnemyZombiesRadius);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
