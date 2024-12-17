using System.Collections.Generic;
using UnityEngine;

public class NecromanceHorde : MonoBehaviour
{
    [HideInInspector] public List<Transform> necromancableZombieHorde = new List<Transform>();

    [SerializeField] private string necromantedZombieName;

    [SerializeField] private LayerMask attackableZombieLayer;
    [SerializeField] private LayerMask ownZombieLayer;
    [SerializeField] private GameObject zombiePrefab;
    public Transform ParentObject;

    public void OnNecromance()
    {
        foreach (Transform zombieHorde in necromancableZombieHorde)
        {
            foreach (Transform zombie in zombieHorde.transform)
            {
                NecromanceZombie(zombie.gameObject);
            }
        }
    }

    public void NecromanceZombie(GameObject necromancableZombie)
    {
        var necromancableZombieCachedData = necromancableZombie.GetComponent<CachedZombieData>();

        necromancableZombieCachedData.DetectNecromanceZombies.enabled = true;
        
        necromancableZombie.transform.parent = ParentObject;
        necromancableZombie.name = necromantedZombieName;
        
        necromancableZombieCachedData.ZombiePlayerHordeRegistry = transform.root.GetComponent<ZombiePlayerHordeRegistry>();
        
        necromancableZombieCachedData.ZombiePlayerHordeRegistry.RegisterZombie(necromancableZombie);
        
        necromancableZombieCachedData.AutoAttack.attackableZombieLayer = attackableZombieLayer;
        //Use this weird function, because if I just set the gameobject.layer equal to the ownZombieLayer, it throws an error trying to set 2 different layers
        necromancableZombieCachedData.AutoAttack.gameObject.layer = Mathf.RoundToInt(Mathf.Log(ownZombieLayer.value, 2));
        necromancableZombieCachedData.AutoAttack.ResetAttack();
        
        //We can get the same zombie layer of this object because the zombie is going to join this zombies team
        necromancableZombieCachedData.ZombieMovement.enabled = true;
        
        necromancableZombieCachedData.SpriteRenderer.sprite = necromancableZombieCachedData.Health.SpriteBeforeDeath;
        necromancableZombieCachedData.SpriteRenderer.enabled = true;
        
        necromancableZombieCachedData.Health.ResetHealth();
        necromancableZombieCachedData.Health.isDead = false;
        necromancableZombieCachedData.Health.IsPlayer = true;
        necromancableZombieCachedData.Health.NecromanceText.SetActive(false);

        necromancableZombieCachedData.Animator.enabled = true;
    }

    public void SpawnPlayerZombies()
    {
        var necromancableZombie = Instantiate(zombiePrefab, ParentObject);

        var necromancableZombieCachedData = necromancableZombie.GetComponent<CachedZombieData>();

        necromancableZombieCachedData.ZombiePlayerHordeRegistry.RegisterZombie(necromancableZombie);

    }
}
