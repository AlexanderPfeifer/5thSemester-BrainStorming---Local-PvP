using System;
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
    [NonSerialized] public bool isAttacking;
    private float currentTimeUntilNextAttack;
    private Transform closestAttackableZombie;

    [Header("Seperation")]
    float updateTimer = 0f;
    Collider[] cachedGroupingZombies;
    [DisplayColor(0, 0, 1), SerializeField] private float attackSeperationRadius;

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
                    if ((zombie.transform.position - transform.position).sqrMagnitude < 
                        (closestAttackableZombie.position - transform.position).sqrMagnitude)
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
        if (closestAttackableZombie == null) 
        { 
            isAttacking = false;
            return;
        }

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

        if (closestAttackableZombie.GetComponent<Health>().isDead ||
            (Physics.Raycast(transform.position, directionToEnemy.normalized, out hit, directionToEnemy.magnitude, 1 << gameObject.layer) && 
            hit.collider.gameObject != gameObject))
        {
            isAttacking = false;
            return; 
        }

        isAttacking = true;

        transform.position = Vector3.MoveTowards(transform.position, (Vector2)closestAttackableZombie.position + SeparationForce(), Time.deltaTime * moveToEnemySpeed);
    }

    Vector2 SeparationForce()
    {
        cachedGroupingZombies = Physics.OverlapSphere(transform.position, attackSeperationRadius * 0.8f, 1 << gameObject.layer);

        Vector2 _separationForce = Vector2.zero;

        // Ignore separation if there's no other zombie nearby and compare to one because overlapCircle always hits itself
        if (cachedGroupingZombies.Length <= 1)
        {
            return _separationForce;
        }

        foreach (Collider zombie in cachedGroupingZombies)
        {
            if (zombie == GetComponent<Collider>())
                continue; 

            Vector2 oppositeDirectionToNearZombie = transform.position - zombie.transform.position;

            // Compare to more than 0 to avoid division by 0
            if (oppositeDirectionToNearZombie.magnitude > 0) 
            {
                _separationForce += oppositeDirectionToNearZombie / oppositeDirectionToNearZombie.magnitude; // Stronger repulsion when closer
            }
        }

        return _separationForce;
    }

    void UpdateCachedZombies()
    {
        if (updateTimer <= 0f)
        {
            cachedGroupingZombies = Physics.OverlapSphere(transform.position, attackSeperationRadius, 1 << gameObject.layer);
            updateTimer = 0.1f; // Update every 0.1s
        }
        updateTimer -= Time.deltaTime;
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
