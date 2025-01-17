using System;
using UnityEngine;

public class CactusDealDamage : MonoBehaviour
{
    [SerializeField] private float dealDamageRange;
    [SerializeField] private LayerMask zombiesLayer;
    [SerializeField] private float maxDamageCooldown;
    private float currentDamageCooldown;
    [SerializeField] private ParticleSystem spikeParticles;

    private void Start() => spikeParticles = GetComponentInChildren<ParticleSystem>();
    
    private void Update()
    {
        if(currentDamageCooldown > 0)
            currentDamageCooldown -= Time.deltaTime;
        
        Collider[] _attackableZombiesHit = Physics.OverlapSphere(transform.position, dealDamageRange, zombiesLayer);

        if (_attackableZombiesHit.Length > 0 && currentDamageCooldown <= 0)
        {
            foreach (var _attackableZombie in _attackableZombiesHit)
            {
                if (_attackableZombie.TryGetComponent(out Health _health))
                {
                    _health.DamageIncome(1, transform);
                }
            }
            
            spikeParticles.Play();
            currentDamageCooldown = maxDamageCooldown;
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dealDamageRange);
    }
}
