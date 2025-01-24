using System;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [Header("IsZombie")]
    [NonSerialized] public bool IsZombie;
    
    [Header("Speed")]
    [SerializeField] private float baseMoveSpeed;
    [SerializeField] private float runMoveSpeed;
    private float currentSpeed;
    
    [Header("Smoothing")]
    [SerializeField] private float speedSmoothTime = 1.0f;
    private Vector3 currentVelocity = new(0, 0, 0);
    
    
    [NonSerialized] public Vector3 MoveDirection;
    
    [Header("Detection")]
    [DisplayColor(0, 1, 0), SerializeField] float detectZombiesRadius;  
    [DisplayColor(1, 0, 1), SerializeField] private float collisionDetectionRadius;
    [SerializeField] private LayerMask zombieLayer;
    
    [Header("Despawn")]
    [SerializeField] float notInCameraRange;
    [SerializeField] private float maxTimeUntilDespawn = 60;
    private float currentTimeUntilDespawn;
    
    [Header("VFX")]
    public ParticleSystem ObtainPointsParticles;

    [Header("Grouping")]
    [SerializeField] private float groupingSpeed;
    [NonSerialized] public PlayerMovement MainZombieMovement;
    
    private CachedZombieData cachedZombieData;

    private void Start()
    {
        currentSpeed = baseMoveSpeed;
        IsZombie = false;
        cachedZombieData = GetComponent<CachedZombieData>();
        currentTimeUntilDespawn = maxTimeUntilDespawn;
        cachedZombieData.Animator.SetFloat("moveSpeed", 5);
    }

    private void Update()
    {
        ZombieDespawnTime();
        
        SetParticlesPosition();

        if (cachedZombieData.AutoAttack.IsAttacking)
            return;

        MoveZombie();
    }

    void SetParticlesPosition()
    {
        //Always set set the particle position to the npc because the particle should follow when being player but it is not a child of this object
        var _particleTransform = ObtainPointsParticles.transform;
        var _npcTransform = transform.position;
        _particleTransform.position = new Vector3(_npcTransform.x, _particleTransform.position.y, _npcTransform.z);
    }
    
    void ZombieDespawnTime()
    {
        if (!IsZombie)
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
        
        //Some behaviour is split up in is zombie and is not because if necromanced the object just takes the human behaviour and modifies it a bit
        switch (IsZombie)
        {
            //Return here because otherwise the zombie is getting stuck on simple objects
            case true when Vector3.Distance(transform.position, MainZombieMovement.transform.position) > MainZombieMovement.groupingRadius:
                GroupWithMainZombie();
                return;
            case false when _zombieHit.Length > 0:
                RunAwayFromZombies(_position, _zombieHit[0].transform.position);
                break;
        }
        
        bool _collisionDetected = Physics.SphereCast(
            transform.position,                   
            collisionDetectionRadius,                   
            MoveDirection.normalized,                 
            out var _hit,                           
            collisionDetectionRadius,         
            ~LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer))// exclude own layer
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
        var _mainZombiePosition = MainZombieMovement.transform.position;
        var _npcPosition = transform.position;
        _npcPosition = Vector3.MoveTowards(_npcPosition, _mainZombiePosition, Time.deltaTime * currentSpeed);
        transform.position = _npcPosition;
        MoveDirection = _mainZombiePosition - _npcPosition;
    }

    private void OnDrawGizmos()
    {
        var _npcPosition = transform.position;
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_npcPosition, detectZombiesRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(_npcPosition, collisionDetectionRadius);
    }
}
