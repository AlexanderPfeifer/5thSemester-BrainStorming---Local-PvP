using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private GameObject smallHordePrefab;
    [SerializeField] private float smallHordeSpawnTime = 3;
    [SerializeField] private SpawnPoint[] spawnPoints;
    [SerializeField] private float zombiesInRangeRadius;
    [SerializeField] private LayerMask zombieLayer;
    [SerializeField] private Transform NPCParent;
        
    private void OnEnable()
    {
        PlayerRegistryManager.Instance.AllPlayersReady += OnSpawnSmallHordesOverTime;
    }

    private void OnDisable()
    {
        PlayerRegistryManager.Instance.AllPlayersReady -= OnSpawnSmallHordesOverTime;
    }

    private void Update()
    {
        foreach (var _spawnPoint in spawnPoints)
        {
            for (int _i = 0; _i < _spawnPoint.spawnNPCCount; _i++)
            {
                if (_spawnPoint.lampLight == null)
                {
                    return;
                }

                _spawnPoint.lampLight.SetActive(!CheckZombiesInRange(_spawnPoint.transform));
            }
        }
    }

    private void OnSpawnSmallHordesOverTime()
    {
        StartCoroutine(SpawnSmallHordesOverTime());
    }

    IEnumerator SpawnSmallHordesOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(smallHordeSpawnTime);

            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        foreach (var _spawnPoint in spawnPoints)
        {
            for (int _i = 0; _i < _spawnPoint.spawnNPCCount; _i++)
            {
                if (CheckZombiesInRange(_spawnPoint.transform))
                {
                    return;
                }
            
                var _horde = Instantiate(smallHordePrefab, _spawnPoint.transform.position + Random.insideUnitSphere, Quaternion.identity, NPCParent);
            
                var _moveDir = Random.insideUnitSphere;
            
                _moveDir.y = 0;
            
                foreach (Transform _child in _horde.transform)
                {
                    _child.GetComponent<NPCMovement>().MoveDirection = _moveDir;
                }     
            }
        }
    }

    bool CheckZombiesInRange(Transform positionToCheck)
    {
        var _zombieHit = Physics.OverlapSphere(positionToCheck.position, zombiesInRangeRadius, zombieLayer);

        if (_zombieHit.Length > 0)
        {
            //Deactivate Light
            return true;
        }

        return false;
    }
    
    private void OnDrawGizmos()
    {
        foreach (var _spawnPoint in spawnPoints)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_spawnPoint.transform.position, zombiesInRangeRadius);   
        }
    }
}
