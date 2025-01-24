using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Health : MonoBehaviour
{
    [Header("Player Toggle")] 
    public bool IsPlayerZombie;

    [Header("Health")]
    [SerializeField] private int maxHealth;
    private int currentHealth;

    [Header("Hit")] 
    [SerializeField] private float changeColorOnHitTime = .3f;
    [HideInInspector] public VisualEffect bloodEffect;

    private CachedZombieData cachedZombieData;

    private void Start()
    {
        ResetHealth();
        cachedZombieData = GetComponent<CachedZombieData>();
    }

    public void DamageIncome(int damageDealt, Transform sender)
    {
        if (!IsPlayerZombie)
            return;
        
        currentHealth -= damageDealt;

        if (currentHealth <= 0)
        {
            var _position = transform.position;
            bloodEffect.transform.position = new Vector3(_position.x, .5f, _position.z);
            bloodEffect.Play();
            
            AudioManager.Instance.PlayWithRandomPitch("ZombieDeath");
            
            Die();
            
            //Resets attack so the NPC does not try to attack in the frame they died
            if (sender.TryGetComponent(out AutoAttack _autoAttack))
            {
                _autoAttack.ResetAttack();
            }
        }
        
        StartCoroutine(ChangeColorOnHitCoroutine());
    }

    private void Die()
    {
        cachedZombieData.ZombiePlayerHordeRegistry.UnregisterZombie(gameObject);
        if (cachedZombieData.NecromanceHorde.zombiesNearBrainPlayer.Contains(transform.GetComponent<Collider>()))
        {
            cachedZombieData.NecromanceHorde.zombiesNearBrainPlayer.Remove(transform.GetComponent<Collider>());
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    private IEnumerator ChangeColorOnHitCoroutine()
    {
        var _material = cachedZombieData.MeshRenderer.material;
        
        _material.color = Color.red;

        yield return new WaitForSeconds(changeColorOnHitTime);
        
        _material.color = Color.white;
    }
}