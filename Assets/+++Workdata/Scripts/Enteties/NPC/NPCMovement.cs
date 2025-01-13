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
    [NonSerialized] public Vector3 moveDirection;
    [DisplayColor(0, 1, 0), SerializeField] float detectZombiesRadius;    
    [SerializeField] float notInCameraRange;
    [SerializeField] private float maxTimeUntilDespawn = 60;
    private float currentTimeUntilDespawn;

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
        currentTimeUntilDespawn = maxTimeUntilDespawn;
    }

    private void Update()
    {
        Collider[] _zombieHit = Physics.OverlapSphere(transform.position, notInCameraRange, zombie);

        if(!isNecromanced)
            currentTimeUntilDespawn -= Time.deltaTime;

        if (currentTimeUntilDespawn <= 0 && _zombieHit.Length <= 0)
        {
            Destroy(gameObject);
        }
        
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
        Collider[] _zombieHit = Physics.OverlapSphere(transform.position, detectZombiesRadius, zombie);

        switch (isNecromanced)
        {
            case true when mainZombieMovement.groupingRadius < Vector2.Distance(transform.position, mainZombieMovement.transform.position):
                GroupWithMainZombie();
                return;
            case false when _zombieHit.Length > 0:
                //Move away from zombies
                currentSpeed = runMoveSpeed;
                transform.position = Vector3.MoveTowards(transform.position, transform.position + (transform.position - _zombieHit[0].transform.position), Time.deltaTime * currentSpeed);
                moveDirection = transform.position + (transform.position - _zombieHit[0].transform.position);
                return;
        }

        Vector3 _moveDirectionNormalized = moveDirection.normalized;

        // Detect collisions in front of the zombie using SphereCast
        bool _zombieDetected = Physics.SphereCast(
            transform.position,                   // Starting point of the cast
            collisionDetectionRadius,             // Radius of the sphere
            _moveDirectionNormalized,              // Direction of the sphere cast
            out var _hit,                              // Output the collision information
            collisionDetectionRadius,             // Max distance for the sphere cast
            ~0                                    // Layer mask (all layers by default)
        );

        if (_zombieDetected)
        {
            currentTimeUntilDespawn = maxTimeUntilDespawn;
            _moveDirectionNormalized = Vector3.Reflect(_moveDirectionNormalized, _hit.normal);
        }
        
        transform.position = Vector3.SmoothDamp(
            transform.position,
            transform.position + (new Vector3(_moveDirectionNormalized.x, 0, _moveDirectionNormalized.z) * currentSpeed) + GetComponent<AutoAttack>().SeparationForce(),
            ref currentVelocity,
            speedSmoothTime
        );
    }

    void GroupWithMainZombie()
    {
        currentSpeed = groupingSpeed;
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
