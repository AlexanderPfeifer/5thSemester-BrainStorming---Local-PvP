using UnityEngine;

public class CachedZombieData : MonoBehaviour
{
    private AutoAttack _autoAttack;
    private ZombieMovement _zombieMovement;
    private Health _health;
    private DetectNecromancableZombies _detectNecromanceZombies;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private NecromanceHorde _necromanceHorde;
    private ZombiePlayerHordeRegistry _zombiePlayerHordeRegistry;
    private ShowNecromanceText _showNecromanceText;

    //I use Lazy Initialization to have both performance - this allows the script to get components only when they are needed, reducing the risk of frame drops
    public AutoAttack AutoAttack => _autoAttack ??= GetComponent<AutoAttack>();
    public ZombieMovement ZombieMovement => _zombieMovement ??= GetComponent<ZombieMovement>();
    public Health Health => _health ??= GetComponent<Health>();
    public DetectNecromancableZombies DetectNecromanceZombies => _detectNecromanceZombies ??= GetComponent<DetectNecromancableZombies>();
    public Animator Animator => _animator ??= GetComponentInChildren<Animator>();
    public SpriteRenderer SpriteRenderer => _spriteRenderer ??= GetComponentInChildren<SpriteRenderer>();
    public NecromanceHorde NecromanceHorde => _necromanceHorde ??= GetComponentInParent<NecromanceHorde>();
    public ZombiePlayerHordeRegistry ZombiePlayerHordeRegistry => _zombiePlayerHordeRegistry ??= GetComponentInParent<ZombiePlayerHordeRegistry>();
    public ShowNecromanceText ShowNecromanceText => _showNecromanceText ??= GetComponent<ShowNecromanceText>();

    //If I ever need components to be cached on initialization, I can use this method
    public void PreCacheComponents()
    {
        _ = AutoAttack;
        _ = ZombieMovement;
        _ = Health;
        _ = DetectNecromanceZombies;
        _ = Animator;
        _ = SpriteRenderer;
        _ = NecromanceHorde;
        _ = ZombiePlayerHordeRegistry;
        _ = ShowNecromanceText;
    }
}
