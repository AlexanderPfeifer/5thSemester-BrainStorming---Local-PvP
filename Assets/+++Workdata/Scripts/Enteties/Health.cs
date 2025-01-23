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

    [Header("Death")]
    private Vector3 startScale;
    private Quaternion startRotation;
    
    private CachedZombieData cachedZombieData;

    private void Start()
    {
        ResetHealth();
        cachedZombieData = GetComponent<CachedZombieData>();
        startRotation = GetComponentInChildren<Transform>().rotation;
        
        if(!IsPlayerZombie)
        {
            startScale = transform.localScale;
        }
    }

    public void DamageIncome(int damageDealt, Transform sender)
    {
        if (!IsPlayerZombie)
            return;
        
        currentHealth -= damageDealt;

        if (currentHealth <= 0)
        {
            bloodEffect.transform.position = new Vector3(transform.position.x, .5f, transform.position.z);
            bloodEffect.Play();
            
            AudioManager.Instance.PlayWithRandomPitch("ZombieDeath");
            Die();
            if(sender.TryGetComponent(out AutoAttack _autoAttack))
                _autoAttack.ResetAttack();
        }
        
        StartCoroutine(ChangeColorOnHitCoroutine());
    }

    private void Die()
    {
        if (!IsPlayerZombie)
        {
            cachedZombieData.Animator.enabled = false;
            
            //Need to reset the animator because it still has effect on the rotation even after disabling it
            cachedZombieData.Animator.Rebind();
            GetComponentInChildren<Transform>().localRotation = startRotation;
            transform.localScale = startScale;
        }
        else
        {
            cachedZombieData.ZombiePlayerHordeRegistry.UnregisterZombie(gameObject);
            if (cachedZombieData.NecromanceHorde.zombiesNearBrainPlayer1.Contains(transform.GetComponent<Collider>()))
            {
                cachedZombieData.NecromanceHorde.zombiesNearBrainPlayer1.Remove(transform.GetComponent<Collider>());
            }
            
            if (cachedZombieData.NecromanceHorde.zombiesNearBrainPlayer2.Contains(transform.GetComponent<Collider>()))
            {
                cachedZombieData.NecromanceHorde.zombiesNearBrainPlayer2.Remove(transform.GetComponent<Collider>());
            }
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    private IEnumerator ChangeColorOnHitCoroutine()
    {
        cachedZombieData.MeshRenderer.material.color = Color.red;

        yield return new WaitForSeconds(changeColorOnHitTime);
        
        cachedZombieData.MeshRenderer.material.color = Color.white;
    }
}