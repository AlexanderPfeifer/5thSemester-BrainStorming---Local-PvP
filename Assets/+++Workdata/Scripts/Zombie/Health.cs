using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] public bool IsPlayer;

    [Header("Health")]
    [SerializeField] private int maxHealth;
    private int currentHealth;

    [Header("Hit")] 
    [SerializeField] private float changeColorOnHitTime = .3f;

    [Header("Death")]
    public Sprite graveSprite;
    [HideInInspector] public bool isDead;
    private Vector3 startScale;

    private CachedZombieData cachedZombieData;

    private void Start()
    {
        ResetHealth();
        cachedZombieData = GetComponent<CachedZombieData>();

        if(!IsPlayer)
        {
            startScale = transform.localScale;
        }
    }

    public void DamageIncome(int damageDealt, AutoAttack autoAttack)
    {
        if (cachedZombieData.SpriteRenderer.sprite == graveSprite)
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
        if (!IsPlayer)
        {
            isDead = true;

            cachedZombieData.SpriteRenderer.sprite = graveSprite;
            cachedZombieData.Animator.enabled = false;
            //Need to reset the animator because it still has effect on the rotation even after disabling it
            cachedZombieData.Animator.Rebind();
            GetComponentInChildren<Transform>().localRotation = Quaternion.identity;
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
        cachedZombieData.SpriteRenderer.color = Color.red;

        yield return new WaitForSeconds(changeColorOnHitTime);
        
        cachedZombieData.SpriteRenderer.color = Color.white;
    }
}

[CustomEditor(typeof(Health))]
public class Health_Editor : Editor
{
    //I created the script because I want to hide variables depending on if the bool IsPlayer is enabled or disabled

    SerializedProperty isPlayerProperty;

    private void OnEnable()
    {
        isPlayerProperty = serializedObject.FindProperty("IsPlayer");
    }

    public override void OnInspectorGUI()
    {
        SerializedProperty property = serializedObject.GetIterator();
        property.NextVisible(true); 

        while (property.NextVisible(false))
        {
            if (property.name == "graveSprite" && isPlayerProperty.boolValue)
                continue;

            EditorGUILayout.PropertyField(property, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}

