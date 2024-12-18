using System.Collections;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private GameObject smallHordePrefab;
    [SerializeField] private float smallHordeSpawnTime = 5;

    private void Start()
    {
        StartCoroutine(SpawnSmallHordesOverTime());
    }

    IEnumerator SpawnSmallHordesOverTime()
    {
        Vector3 spawnPos = Vector3.zero;

        while (true)
        {
            yield return new WaitForSeconds(smallHordeSpawnTime);

            Instantiate(smallHordePrefab, spawnPos, Quaternion.identity);
        }
    }
}
