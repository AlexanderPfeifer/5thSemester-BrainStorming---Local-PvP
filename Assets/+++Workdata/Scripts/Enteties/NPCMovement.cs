using System;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [NonSerialized] public bool isNecromanced;
    [SerializeField] private LayerMask zombie;

    [Header("Movement")]
    private float currentSpeed;
    [SerializeField] private float baseMoveSpeed;
    [SerializeField] private float runMoveSpeed;
    [SerializeField] private float speedSmoothTime = 1.0f;
    private Vector3 lastPosition;
    private Vector3 currentVelocity = new Vector3(0, 0, 0);
    [NonSerialized] public Vector2 moveDirection;
    [DisplayColor(0, 1, 0), SerializeField] float detectZombiesRadius;

    [Header("Grouping")]
    [SerializeField] private float groupingSpeed;
    [NonSerialized] public PlayerMovement mainZombieMovement;
    
    [Header("Collision")]
    [SerializeField] private float collisionDetectionRadius;

    private CachedZombieData cachedZombieData;

    private void Start()
    {
        currentSpeed = baseMoveSpeed;
        isNecromanced = false;
        cachedZombieData = GetComponent<CachedZombieData>();
    }

    private void Update()
    {
        if (cachedZombieData.Health.isDead || cachedZombieData.AutoAttack.isAttacking)
            return;

        MoveZombie();
    }

    private void LateUpdate()
    {
        if (cachedZombieData.Health.isDead)
            return;

        MoveAnimationLateUpdate();
    }

    void MoveZombie()
    {
        Collider[] zombieHit = Physics.OverlapSphere(transform.position, detectZombiesRadius, zombie);

        switch (isNecromanced)
        {
            case true when mainZombieMovement.groupingRadius < Vector2.Distance(transform.position, mainZombieMovement.transform.position):
                GroupWithMainZombie();
                return;
            case false when zombieHit.Length > 0:
                currentSpeed = runMoveSpeed;
                transform.position = Vector2.MoveTowards(transform.position, transform.position + (transform.position - zombieHit[0].transform.position), Time.deltaTime * currentSpeed);
                moveDirection = transform.position + (transform.position - zombieHit[0].transform.position);
                return;
        }

        Vector2 moveDirectionNormalized = moveDirection.normalized;

        // Detect collisions in front of the zombie using SphereCast
        RaycastHit hit;
        bool collisionDetected = Physics.SphereCast(
            transform.position,                   // Starting point of the cast
            collisionDetectionRadius,             // Radius of the sphere
            moveDirectionNormalized,              // Direction of the sphere cast
            out hit,                              // Output the collision information
            collisionDetectionRadius,             // Max distance for the sphere cast
            ~0                                    // Layer mask (all layers by default)
        );

        if (collisionDetected)
        {
            moveDirectionNormalized = Vector2.Reflect(moveDirectionNormalized, hit.normal);
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            (Vector2)transform.position + (moveDirectionNormalized * currentSpeed) + GetComponent<AutoAttack>().SeparationForce(),
            ref currentVelocity,
            speedSmoothTime
        );
    }

    void GroupWithMainZombie()
    {
        transform.position = Vector3.MoveTowards(transform.position, (Vector2)mainZombieMovement.transform.position, Time.deltaTime * currentSpeed);
        moveDirection = mainZombieMovement.transform.position - transform.position;
    }

    void MoveAnimationLateUpdate()
    {
        var currentSpeed = Vector3.Distance(transform.position, lastPosition) / Time.deltaTime;  

        lastPosition = transform.position;

        cachedZombieData.Animator.SetFloat("moveSpeed", 5);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectZombiesRadius);
    }
}
