using UnityEngine;

public class CachedZombieData : MonoBehaviour
{
    private AutoAttack _autoAttack;
    private NPCMovement _npcMovement;
    private Health _health;
    private DetectNecromancableZombies _detectNecromanceZombies;
    private Animator _animator;
    private MeshRenderer _meshRenderer;
    private NecromanceHorde _necromanceHorde;
    private ZombiePlayerHordeRegistry _zombiePlayerHordeRegistry;

    //I use Lazy Initialization to have both performance - this allows the script to get components only when they are needed, reducing the risk of frame drops
    public AutoAttack AutoAttack => _autoAttack ??= GetComponent<AutoAttack>();
    public NPCMovement NPCMovement => _npcMovement ??= GetComponent<NPCMovement>();
    public Health Health => _health ??= GetComponent<Health>();
    public DetectNecromancableZombies DetectNecromanceZombies => _detectNecromanceZombies ??= GetComponent<DetectNecromancableZombies>();
    public Animator Animator => _animator ??= GetComponentInChildren<Animator>();
    public MeshRenderer MeshRenderer => _meshRenderer ??= GetComponentInChildren<MeshRenderer>();
    public NecromanceHorde NecromanceHorde => _necromanceHorde ??= GetComponentInParent<NecromanceHorde>();

    //Make zombiePlayerHordeRegistry as traditional getter/setter to pass a reference inside NecromanceHorde to an NPC
    public ZombiePlayerHordeRegistry ZombiePlayerHordeRegistry
    {
        get => _zombiePlayerHordeRegistry ??= GetComponentInParent<ZombiePlayerHordeRegistry>();
        set => _zombiePlayerHordeRegistry = value;
    }

    //If I ever need components to be cached on initialization, I can use this method
    public void PreCacheComponents()
    {
        _ = AutoAttack;
        _ = NPCMovement;
        _ = Health;
        _ = DetectNecromanceZombies;
        _ = Animator;
        _ = MeshRenderer;
        _ = NecromanceHorde;
        _ = ZombiePlayerHordeRegistry;
    }
}
