using System.Collections.Generic;
using UnityEngine;

public class NecromanceHorde : MonoBehaviour
{
    public HashSet<Transform> NecromancableZombieHorde = new();

    [SerializeField] private string necromantedZombieName;

    [SerializeField] private LayerMask attackableZombieLayer;
    [SerializeField] private LayerMask ownZombieLayer;
    public Transform ParentObject;
    [SerializeField] private Sprite zombieVisual;

    public void OnNecromance()
    {
        var _necromancableHordeSet = new HashSet<Transform>();
        var _necromancableZombieSet = new HashSet<GameObject>();

        // Clear the sets to ensure they are reset each time the method is triggered
        _necromancableHordeSet.Clear();
        _necromancableZombieSet.Clear();
        
        foreach (Transform _zombieHorde in NecromancableZombieHorde)
        {
            _necromancableHordeSet.Add(_zombieHorde);

            foreach(Transform _horde in _necromancableHordeSet)
            {
                foreach (Transform _zombie in _horde)
                {
                    _necromancableZombieSet.Add(_zombie.gameObject);
                }
            }
        }

        NecromancableZombieHorde.Clear();

        foreach(var _zombie in _necromancableZombieSet)
        {
            NecromanceZombie(_zombie);
            _zombie.transform.parent = ParentObject;
        }
    }

    private void NecromanceZombie(GameObject necromancableZombie)
    {
        var _necromancableZombieCachedData = necromancableZombie.GetComponent<CachedZombieData>();

        Destroy(necromancableZombie.transform.parent.GetComponent<ShowNecromanceText>());
        Destroy(_necromancableZombieCachedData.transform.GetComponentInChildren<Canvas>().gameObject);

        necromancableZombie.name = necromantedZombieName;

        _necromancableZombieCachedData.ZombiePlayerHordeRegistry = GetComponent<ZombiePlayerHordeRegistry>();

        _necromancableZombieCachedData.ZombiePlayerHordeRegistry.RegisterZombie(necromancableZombie);
        
        _necromancableZombieCachedData.AutoAttack.attackableZombieLayer = attackableZombieLayer;
        //Use this weird function, because if I just set the gameobject.layer equal to the ownZombieLayer, it throws an error trying to set 2 different layers
        _necromancableZombieCachedData.AutoAttack.gameObject.layer = Mathf.RoundToInt(Mathf.Log(ownZombieLayer.value, 2));
        _necromancableZombieCachedData.AutoAttack.ResetAttack();
        _necromancableZombieCachedData.AutoAttack.enabled = true;

        //We can get the same zombie layer of this object because the zombie is going to join this zombies team
        _necromancableZombieCachedData.NPCMovement.MainZombieMovement = _necromancableZombieCachedData.ZombiePlayerHordeRegistry.mainZombie.GetComponent<PlayerMovement>();
        _necromancableZombieCachedData.NPCMovement.IsNecromanced = true;

        _necromancableZombieCachedData.MeshRenderer.material.mainTexture = zombieVisual.texture;
        
        _necromancableZombieCachedData.Health.ResetHealth();
        _necromancableZombieCachedData.Health.IsDead = false;
        _necromancableZombieCachedData.Health.IsPlayerZombie = true;

        _necromancableZombieCachedData.Animator.enabled = true;
    }
}
