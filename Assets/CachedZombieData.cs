using UnityEngine;

public class CachedZombieData : MonoBehaviour
{
    [HideInInspector] public AutoAttack AutoAttack;
    [HideInInspector] public ZombieMovement ZombieMovement;
    [HideInInspector] public SpriteRenderer SpriteRenderer;
    [HideInInspector] public Health Health;
    [HideInInspector] public Necromance Necromance;
    [HideInInspector] public Animator Animator;

    private void Start()
    {
        AutoAttack = transform.GetComponent<AutoAttack>();
        ZombieMovement = transform.GetComponent<ZombieMovement>();
        Health = transform.GetComponent<Health>();
        Necromance = transform.GetComponent<Necromance>();
        Animator = transform.GetComponentInChildren<Animator>();
        SpriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
    }
}
