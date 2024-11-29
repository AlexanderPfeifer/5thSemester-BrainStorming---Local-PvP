using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth;
    private int currentHealth;

    [Header("Hit")] 
    [SerializeField] private float changeColorOnHitTime = .3f;
    [SerializeField] private SpriteRenderer sr;
    
    [Header("Death")]
    [HideInInspector] public bool isDead;
    [SerializeField] private bool isPlayer;
    [SerializeField] private LayerMask graveLayer;
    [SerializeField] private Sprite graveSprite;

    private void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void DamageIncome(int damageDealt)
    {
        currentHealth -= damageDealt;

        if (currentHealth <= 0)
        {
            Die();
        }
        
        StartCoroutine(ChangeColorOnHitCoroutine());
    }

    private void Die()
    {
        if (!isPlayer)
        {
            isDead = true;
            sr.sprite = graveSprite;
            GetComponentInChildren<Animator>().enabled = false;
            gameObject.layer = LayerMask.NameToLayer("Grave"); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator ChangeColorOnHitCoroutine()
    {
        if (sr.enabled)
        {
            sr.color = Color.red;

            yield return new WaitForSeconds(changeColorOnHitTime);
        
            sr.color = Color.white;   
        }
    }
}
