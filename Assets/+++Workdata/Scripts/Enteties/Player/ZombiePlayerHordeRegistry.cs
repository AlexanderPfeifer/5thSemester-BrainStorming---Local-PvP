using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class ZombiePlayerHordeRegistry : MonoBehaviour
{
    public List<GameObject> Zombies { get; } = new();

    [Header("Multiplayer")]
    [SerializeField] private int playerIndex;

    [Header("Movement")]
    [NonSerialized] public GameObject MainZombie;
    
    [Header("GameStart")]
    [SerializeField] private GameObject zombiePrefab;
    [FormerlySerializedAs("removableZombieAfterSpawn")] [SerializeField] private GameObject grave;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera cineCam;

    public void RegisterZombie(GameObject zombie)
    {
        if(!Zombies.Contains(zombie))
        {
            if (MainZombie == null)
            {
                MainZombie = zombie;
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
        var _necromanceHorde = GetComponent<NecromanceHorde>();
        var _position = transform.position;
        
        var _player = Instantiate(zombiePrefab, 
            new Vector3(_position.x, _position.y, _position.z), Quaternion.identity, _necromanceHorde.ParentObject);
        
        RegisterZombie(_player);
        
        cineCam.Target.TrackingTarget = _player.transform;

        //Check for player index to destroy the right grave according to the player 
        if (playerIndex == 0)
        {
            _necromanceHorde.interactCircle.transform.position = grave.transform.position;
            _necromanceHorde.interactCircle.Play();
            Destroy(grave);
        }
        else
        {
            _necromanceHorde.interactCircle.transform.position = grave.transform.position;
            _necromanceHorde.interactCircle.Play();
            Destroy(grave);
        }
    }

    public int GetPlayerIndex()
    { 
        return playerIndex; 
    }
}
