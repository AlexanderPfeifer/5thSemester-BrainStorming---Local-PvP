using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private GameObject smallHordePrefab;
    [SerializeField] private float smallHordeSpawnTime = 3;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private VisualEffect bloodEffect;

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
            var _horde = Instantiate(smallHordePrefab, _spawnPoint.transform.position, Quaternion.identity, transform);
            
            var _moveDir = Random.insideUnitSphere;
            
            _moveDir.y = 0;
            
            foreach (Transform _child in _horde.transform)
            {
                _child.GetComponent<NPCMovement>().MoveDirection = _moveDir;
            }
        }
    }
}
