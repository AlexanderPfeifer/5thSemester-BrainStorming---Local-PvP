using System;
using UnityEngine;
using UnityEngine.Serialization;

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

    [Header("Separation")]
    float updateTimer;
    Collider[] cachedGroupingZombies;
    [DisplayColor(0, 0, 1), SerializeField] private float attackSeparationRadius;

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
        Collider[] _attackableZombieHit = Physics.OverlapSphere(transform.position, detectEnemyZombiesRadius, attackableZombieLayer);

        if (_attackableZombieHit.Length > 0)
        {
            if (!isAttacking)
            {
                foreach (Collider _zombie in _attackableZombieHit)
                {
                    if (_zombie.TryGetComponent(out NPCMovement _npcMovement))
                    {
                        closestAttackableZombie = _attackableZombieHit[0].transform;
                            
                        // Compare squared distances to avoid unnecessary square root calculations
                        if ((_zombie.transform.position - transform.position).sqrMagnitude < 
                            (closestAttackableZombie.position - transform.position).sqrMagnitude) 
                        {
                            closestAttackableZombie = _zombie.transform;
                        }
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
        Vector3 _directionToEnemy = closestAttackableZombie.position - transform.position;

        if (closestAttackableZombie.GetComponent<Health>().isDead ||
            (Physics.Raycast(transform.position, _directionToEnemy.normalized, out var _hit, _directionToEnemy.magnitude, 1 << gameObject.layer) && 
             _hit.collider.gameObject != gameObject))
        {
            isAttacking = false;
            return; 
        }

        isAttacking = true;

        transform.position = Vector3.MoveTowards(transform.position, closestAttackableZombie.position + SeparationForce(), Time.deltaTime * moveToEnemySpeed);
    }

    public Vector3 SeparationForce()
    {
        cachedGroupingZombies = Physics.OverlapSphere(transform.position, attackSeparationRadius * 0.8f, 1 << gameObject.layer);

        Vector3 _separationForce = Vector3.zero;

        // Ignore separation if there's no other zombie nearby and compare to one because overlapCircle always hits itself
        if (cachedGroupingZombies.Length <= 1)
        {
            return _separationForce;
        }

        foreach (Collider _zombie in cachedGroupingZombies)
        {
            if (_zombie == GetComponent<Collider>())
                continue; 

            Vector3 _oppositeDirectionToNearZombie = transform.position - _zombie.transform.position;
            _oppositeDirectionToNearZombie.y = 0;

            // Compare to more than 0 to avoid division by 0
            if (_oppositeDirectionToNearZombie.magnitude > 0) 
            {
                _separationForce += _oppositeDirectionToNearZombie / _oppositeDirectionToNearZombie.magnitude; // Stronger repulsion when closer
            }
        }

        return _separationForce;
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
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackSeparationRadius);
    }
}
