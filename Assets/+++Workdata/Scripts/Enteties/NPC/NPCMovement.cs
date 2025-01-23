using System;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float baseMoveSpeed;
    [SerializeField] private float runMoveSpeed;
    [SerializeField] private float speedSmoothTime = 1.0f;
    private float currentSpeed;
    private Vector3 lastPosition;
    private Vector3 currentVelocity = new(0, 0, 0);
    [NonSerialized] public Vector3 MoveDirection;
    [DisplayColor(0, 1, 0), SerializeField] float detectZombiesRadius;    
    [SerializeField] float notInCameraRange;
    [SerializeField] private float maxTimeUntilDespawn = 60;
    private float currentTimeUntilDespawn;
    public ParticleSystem ObtainPointsParticles;

    [Header("Grouping")]
    [SerializeField] private float groupingSpeed;
    [NonSerialized] public bool IsNecromanced;
    [NonSerialized] public PlayerMovement MainZombieMovement;
    
    [Header("Collision")]
    [SerializeField] private LayerMask zombieLayer;
    [DisplayColor(1, 0, 1), SerializeField] private float collisionDetectionRadius;

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

        ObtainPointsParticles.transform.position = new Vector3(transform.position.x, ObtainPointsParticles.transform.position.y, transform.position.z);
        
        if (cachedZombieData.AutoAttack.IsAttacking)
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
                Destroy(transform.parent.gameObject);
            }
        }
    }

    void MoveZombie()
    {
        var _position = transform.position;

        Collider[] _zombieHit = Physics.OverlapSphere(_position, detectZombiesRadius, zombieLayer);
        
        switch (IsNecromanced)
        {
            case true when Vector3.Distance(transform.position, MainZombieMovement.transform.position) > MainZombieMovement.groupingRadius:
                GroupWithMainZombie();
                return;
            case false when _zombieHit.Length > 0:
                RunAwayFromZombies(_position, _zombieHit[0].transform.position);
                break;
        }
        
        int _layerMask = ~LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));
        
        // Detect collisions in front of the zombie using SphereCast
        bool _collisionDetected = Physics.SphereCast(
            transform.position,                   
            collisionDetectionRadius,                   
            MoveDirection.normalized,                 
            out var _hit,                           
            collisionDetectionRadius,         
            _layerMask                               // exclude own layer
        );

        if (_collisionDetected) 
        {
            MoveDirection = _hit.normal;
        }
        
        Vector3 _moveDirectionNormalized = MoveDirection.normalized;

        transform.position = Vector3.SmoothDamp(_position, 
            _position + (new Vector3(_moveDirectionNormalized.x, 0, _moveDirectionNormalized.z) * currentSpeed)
                      + GetComponent<AutoAttack>().SeparationForce(), ref currentVelocity, speedSmoothTime);
    }

    void RunAwayFromZombies(Vector3 position, Vector3 zombiePosition)
    {
        currentTimeUntilDespawn = maxTimeUntilDespawn;
        currentSpeed = runMoveSpeed;
        AudioManager.Instance.PlayWithRandomPitch("HumanShocked");
        MoveDirection = new Vector3(position.x, 0, position.z) - new Vector3(zombiePosition.x, 0, zombiePosition.z);
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

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, collisionDetectionRadius);
    }
}
