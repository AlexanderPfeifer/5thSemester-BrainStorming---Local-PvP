using System;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [NonSerialized] public bool isNecromanced;

    [Header("Movement")]
    [SerializeField] private float baseMoveSpeed;
    [SerializeField] private float speedSmoothTime = 1.0f;
    private Vector3 lastPosition;
    private Vector3 currentVelocity = new Vector3(0, 0, 0);
    [NonSerialized] public Vector2 moveDirection;

    [Header("Grouping")]
    [SerializeField] private float groupingSpeed;
    [NonSerialized] public PlayerMovement mainZombieMovement;

    [Header("Collision")]
    [SerializeField] private float collisionDetectionRadius;

    private CachedZombieData cachedZombieData;

    private void Start()
    {
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
        if(isNecromanced && mainZombieMovement.groupingRadius < Vector2.Distance(transform.position, mainZombieMovement.transform.position))
        {
            GroupWithMainZombie();
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

        Debug.Log(collisionDetected);

        if (collisionDetected)
        {
            moveDirectionNormalized = Vector2.Reflect(moveDirectionNormalized, hit.normal);
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            (Vector2)transform.position + (moveDirectionNormalized * baseMoveSpeed),
            ref currentVelocity,
            speedSmoothTime
        );
    }

    void GroupWithMainZombie()
    {
        transform.position = Vector3.MoveTowards(transform.position, (Vector2)mainZombieMovement.transform.position, Time.deltaTime * baseMoveSpeed);
        moveDirection = mainZombieMovement.transform.position - transform.position;
    }

    void MoveAnimationLateUpdate()
    {
        var currentSpeed = Vector3.Distance(transform.position, lastPosition) / Time.deltaTime;  

        lastPosition = transform.position;

        cachedZombieData.Animator.SetFloat("moveSpeed", currentSpeed);
    }
}
