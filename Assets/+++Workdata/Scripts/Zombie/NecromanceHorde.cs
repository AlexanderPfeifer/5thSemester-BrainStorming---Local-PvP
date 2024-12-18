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
    [SerializeField] private Sprite zombieVisual;

    public void OnNecromance()
    {
        var necromancableHordeSet = new HashSet<Transform>();
        var necromancedZombie = new HashSet<GameObject>();

        foreach (Transform zombieHorde in necromancableZombieHorde)
        {
            foreach(Transform zombie in zombieHorde)
            {
                NecromanceZombie(zombie.gameObject);
                necromancedZombie.Add(zombie.gameObject);
            }
  
            necromancableHordeSet.Add(zombieHorde);
        }

        foreach (var horde in necromancableHordeSet)
        {
            necromancableZombieHorde.Remove(horde);
        }

        foreach(var zombie in necromancedZombie)
        {
            zombie.transform.parent = ParentObject;
        }
    }

    public void NecromanceZombie(GameObject necromancableZombie)
    {
        var necromancableZombieCachedData = necromancableZombie.GetComponent<CachedZombieData>();

        necromancableZombieCachedData.DetectNecromanceZombies.enabled = true;

        Destroy(necromancableZombie.transform.parent.GetComponent<ShowNecromanceText>());
        Destroy(necromancableZombieCachedData.transform.GetComponentInChildren<Canvas>().gameObject);

        necromancableZombie.name = necromantedZombieName;

        necromancableZombieCachedData.ZombiePlayerHordeRegistry = GetComponent<ZombiePlayerHordeRegistry>();

        necromancableZombieCachedData.ZombiePlayerHordeRegistry.RegisterZombie(necromancableZombie);
        
        necromancableZombieCachedData.AutoAttack.attackableZombieLayer = attackableZombieLayer;
        //Use this weird function, because if I just set the gameobject.layer equal to the ownZombieLayer, it throws an error trying to set 2 different layers
        necromancableZombieCachedData.AutoAttack.gameObject.layer = Mathf.RoundToInt(Mathf.Log(ownZombieLayer.value, 2));
        necromancableZombieCachedData.AutoAttack.ResetAttack();
        
        //We can get the same zombie layer of this object because the zombie is going to join this zombies team
        necromancableZombieCachedData.ZombieMovement.enabled = true;
        
        necromancableZombieCachedData.SpriteRenderer.sprite = zombieVisual;
        necromancableZombieCachedData.SpriteRenderer.enabled = true;
        
        necromancableZombieCachedData.Health.ResetHealth();
        necromancableZombieCachedData.Health.isDead = false;
        necromancableZombieCachedData.Health.IsPlayer = true;

        necromancableZombieCachedData.Animator.enabled = true;
    }

    public void SpawnPlayerZombies()
    {
        var necromancableZombie = Instantiate(zombiePrefab, ParentObject);

        var necromancableZombieCachedData = necromancableZombie.GetComponent<CachedZombieData>();

        necromancableZombieCachedData.ZombiePlayerHordeRegistry.RegisterZombie(necromancableZombie);

    }
}
