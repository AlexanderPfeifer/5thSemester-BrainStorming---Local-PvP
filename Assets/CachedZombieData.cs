using UnityEngine;

public class CachedZombieData : MonoBehaviour
{
    [HideInInspector] public AutoAttack AutoAttack;
    [HideInInspector] public ZombieMovement ZombieMovement;
    [HideInInspector] public SpriteRenderer SpriteRenderer;
    [HideInInspector] public Health Health;
    [HideInInspector] public DetectNecromancableZombies DetectNecromanceZombies;
    [HideInInspector] public Animator Animator;
    [HideInInspector] public NecromanceHorde NecromanceHorde;
    [HideInInspector] public ZombiePlayerHordeRegistry ZombiePlayerHordeRegistry;

    private void OnEnable()
    {
        AutoAttack = GetComponent<AutoAttack>();
        ZombieMovement = GetComponent<ZombieMovement>();
        Health = GetComponent<Health>();
        DetectNecromanceZombies = GetComponent<DetectNecromancableZombies>();
        Animator = GetComponentInChildren<Animator>();
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        NecromanceHorde = GetComponentInParent<NecromanceHorde>();
        ZombiePlayerHordeRegistry = GetComponentInParent<ZombiePlayerHordeRegistry>();
    }
}
