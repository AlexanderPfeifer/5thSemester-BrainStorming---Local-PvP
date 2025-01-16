using System.Collections;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private GameObject smallHordePrefab;
    [SerializeField] private float smallHordeSpawnTime = 3;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float zombiesInRangeRadius;
    [SerializeField] private LayerMask zombieLayer;
    
    private void OnEnable()
    {
        PlayerRegistryManager.Instance.AllPlayersReady += OnSpawnSmallHordesOverTime;
    }

    private void OnDisable()
    {
        PlayerRegistryManager.Instance.AllPlayersReady -= OnSpawnSmallHordesOverTime;
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
            if (CheckZombiesInRange(_spawnPoint))
            {
                return;
            }
            
            var _horde = Instantiate(smallHordePrefab, _spawnPoint.transform.position, Quaternion.identity, transform);
            
            var _moveDir = Random.insideUnitSphere;
            
            _moveDir.y = 0;
            
            foreach (Transform _child in _horde.transform)
            {
                _child.GetComponent<NPCMovement>().MoveDirection = _moveDir;
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
            Gizmos.DrawWireSphere(_spawnPoint.position, zombiesInRangeRadius);   
        }
    }
}
