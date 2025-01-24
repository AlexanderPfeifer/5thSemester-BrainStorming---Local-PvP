using System;
using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    
    [Header("Attack Time")]
    [SerializeField] private float maxTimeUntilNextAttack;
    private float currentTimeUntilNextAttack;
    
    [Header("Attack Detection")]
    [SerializeField] public LayerMask attackableZombieLayer;
    [DisplayColor(1, 0, 0), SerializeField] private float attackRadius;
    [NonSerialized] public bool IsAttacking;
    
    [Header("Attack Damage")]
    [SerializeField] private int damage;
    
    [Header("Enemy Detection")]
    [SerializeField] private float moveToEnemySpeed;
    [DisplayColor(1, 1, 0), SerializeField] private float detectEnemyZombiesRadius;
    private Transform closestAttackableZombie;

    [Header("Attack Separation")]
    [DisplayColor(0, 0, 1), SerializeField] private float attackSeparationRadius;
    Collider[] cachedGroupingZombies;

    [Header("VFX")]
    [SerializeField] private ParticleSystem turnedToZombie;

    [HideInInspector] public CachedZombieData cachedZombieData;

    private void Awake()
    {
        cachedZombieData = GetComponent<CachedZombieData>();
    }

    private void OnEnable()
    {
        var _turnedToZombie = turnedToZombie.main;
        var _interactCircle = cachedZombieData.NecromanceHorde.interactCircle.main;
        _turnedToZombie.startColor = _interactCircle.startColor;
        
        turnedToZombie.Play();
    }

    private void Update()
    {
        IdentifyAttackableZombie();
    }

    private void IdentifyAttackableZombie()
    {
        Collider[] _attackableZombieHit = Physics.OverlapSphere(transform.position, detectEnemyZombiesRadius, attackableZombieLayer);

        if (_attackableZombieHit.Length > 0)
        {
            if (!IsAttacking)
            {
                foreach (Collider _zombie in _attackableZombieHit)
                {
                    if (_zombie.TryGetComponent(out NPCMovement _))
                    {
                        closestAttackableZombie = _zombie.transform;
                            
                        // Compare squared distances to avoid unnecessary square root calculations
                        if ((_zombie.transform.position - transform.position).sqrMagnitude < (closestAttackableZombie.position - transform.position).sqrMagnitude) 
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
            IsAttacking = false;
        }
    }
    
    private void HandleAttackBehaviour()
    {
        if (closestAttackableZombie == null) 
        { 
            IsAttacking = false;
            return;
        }

        if (Vector3.Distance(transform.position, closestAttackableZombie.position) < attackRadius)
        {
            AttackEnemy();
        }
        else
        {
            MoveTowardsClosestEnemy();
        }
    }

    private void AttackEnemy()
    {
        currentTimeUntilNextAttack -= Time.deltaTime;

        if (currentTimeUntilNextAttack <= 0)
        {
            AudioManager.Instance.PlayWithRandomPitch("ZombieBite");
            cachedZombieData.Animator.SetTrigger("attack");
            currentTimeUntilNextAttack = maxTimeUntilNextAttack;
        }
    }

    private void MoveTowardsClosestEnemy()
    {
        Vector3 _directionToEnemy = closestAttackableZombie.position - transform.position;
        if (closestAttackableZombie == null || (Physics.Raycast(
                transform.position, _directionToEnemy.normalized, out var _hit, _directionToEnemy.magnitude, 
                1 << gameObject.layer) && _hit.collider.gameObject != gameObject))
        {
            IsAttacking = false;
            return; 
        }

        IsAttacking = true;

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
        if (closestAttackableZombie != null)
        {
            closestAttackableZombie.GetComponent<Health>().DamageIncome(damage, transform);
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
