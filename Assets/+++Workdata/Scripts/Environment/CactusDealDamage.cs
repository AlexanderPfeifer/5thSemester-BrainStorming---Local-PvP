using UnityEngine;

public class CactusDealDamage : MonoBehaviour
{
    [Header("HitDetection")]
    [SerializeField] private float dealDamageRange;
    [SerializeField] private LayerMask zombiesLayer;
    
    [Header("Cooldown")]
    [SerializeField] private float maxDamageCooldown;
    private float currentDamageCooldown;
    
    [Header("VFX")]
    private ParticleSystem spikeParticles;

    private void Start()
    {
        spikeParticles = GetComponentInChildren<ParticleSystem>();
    }
    
    private void Update()
    {
        ThrowSpikesAfterTime();
    }

    private void ThrowSpikesAfterTime()
    {
        if (currentDamageCooldown > 0)
        {
            currentDamageCooldown -= Time.deltaTime;
        }
        else
        {
            Collider[] _attackableZombiesHit = Physics.OverlapSphere(transform.position + Vector3.up, dealDamageRange, zombiesLayer);

            foreach (var _attackableZombie in _attackableZombiesHit)
            {
                if (_attackableZombie.TryGetComponent(out Health _health))
                {
                    _health.DamageIncome(1, transform);
                    AudioManager.Instance.PlayWithRandomPitch("KaktusHit");
                }
            }
            
            spikeParticles.Play();
            
            currentDamageCooldown = maxDamageCooldown;
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, dealDamageRange);
    }
}
