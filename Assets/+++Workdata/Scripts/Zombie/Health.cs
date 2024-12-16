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
    [SerializeField] public bool isPlayer;
    [SerializeField] private Sprite graveSprite;
    [HideInInspector] public bool isDead;
    private Vector3 startScale;

    private void Start()
    {
        ResetHealth();
        sr = GetComponentInChildren<SpriteRenderer>();
        startScale = transform.localScale;
    }

    public void DamageIncome(int damageDealt, AutoAttack autoAttack)
    {
        if(sr.sprite == graveSprite)
            return;
        
        currentHealth -= damageDealt;

        if (currentHealth <= 0)
        {
            Die();
            autoAttack.ResetAttack();
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
            //Need to reset the animator because it still has effect on the rotation even after disabling it
            GetComponentInChildren<Animator>().Rebind();
            GetComponentInChildren<Transform>().localRotation = Quaternion.identity;
            transform.localScale = startScale;

            gameObject.layer = LayerMask.NameToLayer("Grave");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    private IEnumerator ChangeColorOnHitCoroutine()
    {
        sr.color = Color.red;

        yield return new WaitForSeconds(changeColorOnHitTime);
        
        sr.color = Color.white;
    }
}
