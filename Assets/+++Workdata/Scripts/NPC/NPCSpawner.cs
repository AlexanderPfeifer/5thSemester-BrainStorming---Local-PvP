using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private GameObject smallHordePrefab;
    [SerializeField] private float smallHordeSpawnTime = 3;
    [SerializeField] private float spawnDistance = 10f;
    [SerializeField] Camera[] playerCams;

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
        foreach (var cam in playerCams)
        {
            //Subtract .5 of the distanceFromCamera because otherwise the NPCs clip half inside and hald outside the cameraFarClipPlane
            float distanceFromCamera = cam.farClipPlane - .5f;

            Vector3[] frustumCornerCoordinates = GetFrustumCorners(distanceFromCamera, cam);

            Vector3 spawnPosition = GetRandomPositionOfFrustum(frustumCornerCoordinates);

            var horde = Instantiate(smallHordePrefab, spawnPosition, Quaternion.identity, transform);
            
            var moveDir = cam.transform.position - spawnPosition;

            moveDir.z = 0;
            
            foreach (Transform child in horde.transform)
            {
                child.GetComponent<NPCMovement>().moveDirection = moveDir;
            }
        }
    }

    Vector3[] GetFrustumCorners(float distance, Camera cam)
    {
        Vector3[] corners = new Vector3[4];
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), distance, Camera.MonoOrStereoscopicEye.Mono, corners);

        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = cam.transform.TransformPoint(corners[i]);
        }

        return corners;
    }

    Vector3 GetRandomPositionOfFrustum(Vector3[] frustumCorners)
    {
        int edgeIndex1 = Random.Range(0, 4);
        int edgeIndex2 = (edgeIndex1 + 1) % 4;

        Vector3 edgePoint = Vector3.Lerp(frustumCorners[edgeIndex1], frustumCorners[edgeIndex2], Random.Range(0f, 1f));

        Vector3 frustumCenter = (frustumCorners[0] + frustumCorners[1] + frustumCorners[2] + frustumCorners[3]) / 4;

        return edgePoint + (edgePoint - frustumCenter).normalized * spawnDistance;
    }
}
