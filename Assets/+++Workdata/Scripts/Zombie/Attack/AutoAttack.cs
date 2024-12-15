using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    [Header("Speed")] 
    [SerializeField] private float moveToEnemySpeed;

    [Header("Animation")]
    private Animator animator;
    
    [Header("Attack")]
    [SerializeField] public LayerMask attackableZombieLayer;
    [DisplayColor(1, 0, 0), SerializeField] private float attackRadius;
    [SerializeField] private int damage;
    [DisplayColor(1, 1, 0), SerializeField] private float detectEnemyZombiesRadius;
    [SerializeField] private float maxTimeUntilNextAttack;
    [HideInInspector] public bool isAttacking;
    private float currentTimeUntilNextAttack;
    private Transform closestAttackableZombie;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if(GetComponent<Health>().isDead)
            return;
        
        IdentifyAttackableZombie();
    }

    private void IdentifyAttackableZombie()
    {
        Collider2D[] attackableZombieHit = Physics2D.OverlapCircleAll(transform.position, detectEnemyZombiesRadius, attackableZombieLayer);

        if (attackableZombieHit.Length > 0)
        {
            if(!isAttacking)
            {
                closestAttackableZombie = attackableZombieHit[0].transform; // Start with the first zombie

                foreach (Collider2D zombie in attackableZombieHit)
                {
                    if ((zombie.transform.position - transform.position).sqrMagnitude < (closestAttackableZombie.position - transform.position).sqrMagnitude)
                    {
                        closestAttackableZombie = zombie.transform;
                    }
                }
            }

            Attack();
        }
    }
    
    private void Attack()
    {
        if ((transform.position - closestAttackableZombie.position).sqrMagnitude < attackRadius && closestAttackableZombie.GetComponent<Health>().isDead == false)
        {
            isAttacking = true;

            currentTimeUntilNextAttack -= Time.deltaTime;

            if (currentTimeUntilNextAttack < 0)
            {
                animator.SetTrigger("attack");
                currentTimeUntilNextAttack = maxTimeUntilNextAttack;
            }
        }
        else
        {
            isAttacking = false;

            transform.position = Vector3.MoveTowards(transform.position, closestAttackableZombie.position, Time.deltaTime * moveToEnemySpeed);
        }
    }

    public void AttackEnemyAnimationEvent()
    {
        if (closestAttackableZombie != null)
        {
            closestAttackableZombie.GetComponent<Health>().DamageIncome(damage);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectEnemyZombiesRadius);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
