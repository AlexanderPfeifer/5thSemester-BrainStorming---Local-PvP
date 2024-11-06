using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private Sprite zombieSprite;
    private int currentHealth;
    [SerializeField] private int maxHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void DamageIncome(GameObject zombieDealingDamage, int damageDealt)
    {
        currentHealth -= damageDealt;

        StartCoroutine(ChangeColorToRedOnHitCoroutine());
        
        if (currentHealth <= 0)
        {
            GetComponent<ZombieAutoAttack>().enabled = true;
            gameObject.layer = zombieDealingDamage.layer;
            GetComponentInChildren<SpriteRenderer>().sprite = zombieSprite;
            transform.SetParent(zombieDealingDamage.transform.parent);
        }
    }

    private IEnumerator ChangeColorToRedOnHitCoroutine()
    {
        GetComponentInChildren<SpriteRenderer>().color = Color.red;

        yield return new WaitForSeconds(.3f);
        
        GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }
}
