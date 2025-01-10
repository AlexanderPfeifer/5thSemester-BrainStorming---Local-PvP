using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class ZombiePlayerHordeRegistry : MonoBehaviour
{
    public List<GameObject> Zombies { get; private set; } = new List<GameObject>();

    [SerializeField] private int playerIndex;
    [NonSerialized] public NecromanceHorde necromanceHorde;
    
    [NonSerialized] public GameObject mainZombie;
    
    [SerializeField] private GameObject zombiePrefab;

    [SerializeField] private CinemachineCamera cineCam;

    private void Start()
    {
        necromanceHorde = GetComponent<NecromanceHorde>();
    }

    public void RegisterZombie(GameObject zombie)
    {
        if(!Zombies.Contains(zombie))
        {
            if (mainZombie == null)
            {
                mainZombie = zombie;
            }
            Zombies.Add(zombie);
        }
    }

    public void UnregisterZombie(GameObject zombie)
    {
        if (Zombies.Contains(zombie))
        {
            Zombies.Remove(zombie);

            Destroy(zombie);
        }
    }

    public void SpawnPlayerZombie()
    {
        var _player = Instantiate(zombiePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity, necromanceHorde.ParentObject);
        RegisterZombie(_player);
        cineCam.Target.TrackingTarget = _player.transform;
    }

    public int GetPlayerIndex()
    { 
        return playerIndex; 
    }
}
