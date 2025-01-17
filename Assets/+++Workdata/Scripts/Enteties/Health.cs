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
    [SerializeField] private VisualEffect bloodEffect;

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
        
        Vector3 _enemy = sender.position;
        _enemy.y = 0f;

        Vector3 _player = transform.position;
        _enemy.x -= _player.x;
        _enemy.z -= _player.z;

        float _angle = Mathf.Atan2(_enemy.x, _enemy.z) * Mathf.Rad2Deg;
        
        bloodEffect.transform.rotation = Quaternion.Euler(new Vector3(0, 0, _angle));
        bloodEffect.transform.position = new Vector3(transform.position.x, .5f, transform.position.z);
        bloodEffect.Play();

        if (currentHealth <= 0)
        {
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

            gameObject.layer = LayerMask.NameToLayer("Grave");
        }
        else
        {
            cachedZombieData.ZombiePlayerHordeRegistry.UnregisterZombie(gameObject);
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