using System;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [NonSerialized] public bool IsNecromanced;
    [SerializeField] private LayerMask zombieLayer;

    [Header("Movement")]
    private float currentSpeed;
    [SerializeField] private float baseMoveSpeed;
    [SerializeField] private float runMoveSpeed;
    [SerializeField] private float speedSmoothTime = 1.0f;
    private Vector3 lastPosition;
    private Vector3 currentVelocity = new(0, 0, 0);
    [NonSerialized] public Vector3 MoveDirection;
    [DisplayColor(0, 1, 0), SerializeField] float detectZombiesRadius;    
    [SerializeField] float notInCameraRange;
    [SerializeField] private float maxTimeUntilDespawn = 60;
    private float currentTimeUntilDespawn;

    [Header("Grouping")]
    [SerializeField] private float groupingSpeed;
    [NonSerialized] public PlayerMovement MainZombieMovement;
    
    [Header("Collision")]
    [SerializeField] private float collisionDetectionRadius;

    private CachedZombieData cachedZombieData;

    private void Start()
    {
        currentSpeed = baseMoveSpeed;
        IsNecromanced = false;
        cachedZombieData = GetComponent<CachedZombieData>();
        currentTimeUntilDespawn = maxTimeUntilDespawn;
        cachedZombieData.Animator.SetFloat("moveSpeed", 5);
    }

    private void Update()
    {
        ZombieDespawnTime();
        
        if (cachedZombieData.Health.IsDead || cachedZombieData.AutoAttack.IsAttacking)
            return;

        MoveZombie();
    }

    void ZombieDespawnTime()
    {
        if (!IsNecromanced)
        {
            Collider[] _zombieHit = Physics.OverlapSphere(transform.position, notInCameraRange, zombieLayer);

            currentTimeUntilDespawn -= Time.deltaTime;

            if (currentTimeUntilDespawn <= 0 && _zombieHit.Length <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    void MoveZombie()
    {
        var _position = transform.position;

        Collider[] _zombieHit = Physics.OverlapSphere(_position, detectZombiesRadius, zombieLayer);
        
        switch (IsNecromanced)
        {
            case true when Vector2.Distance(transform.position, MainZombieMovement.transform.position) > MainZombieMovement.groupingRadius:
                GroupWithMainZombie();
                return;
            case false when _zombieHit.Length > 0:
                //Move away from zombies
                currentSpeed = runMoveSpeed;
                
                _position = Vector3.MoveTowards(_position, 
                    _position + (_position - _zombieHit[0].transform.position), 
                    Time.deltaTime * currentSpeed);
                
                transform.position = _position;
                
                MoveDirection = _position + (_position - _zombieHit[0].transform.position);
                return;
        }

        Vector3 _moveDirectionNormalized = MoveDirection.normalized;

        // Detect collisions in front of the zombie using SphereCast
        bool _collisionDetected = Physics.SphereCast(
            transform.position,                    // Starting point of the cast
            collisionDetectionRadius,                   // Radius of the sphere
            _moveDirectionNormalized,                   // Direction of the sphere cast
            out var _hit,                               // Output the collision information
            collisionDetectionRadius,         // Max distance for the sphere cast
            ~0                                  // Layer mask (all layers by default)
        );

        if (_collisionDetected) 
        {
            currentTimeUntilDespawn = maxTimeUntilDespawn;
            _moveDirectionNormalized = Vector3.Reflect(_moveDirectionNormalized, _hit.normal);
        }
        
        transform.position = Vector3.SmoothDamp(_position, 
            _position + (new Vector3(_moveDirectionNormalized.x, 0, _moveDirectionNormalized.z) * currentSpeed)
                      + GetComponent<AutoAttack>().SeparationForce(), ref currentVelocity, speedSmoothTime);
    }

    void GroupWithMainZombie()
    {
        currentSpeed = groupingSpeed;
        transform.position = Vector3.MoveTowards(transform.position, MainZombieMovement.transform.position, Time.deltaTime * currentSpeed);
        MoveDirection = MainZombieMovement.transform.position - transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectZombiesRadius);
    }
}
