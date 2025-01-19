using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera[] targetCamera;
    private Transform currentCamFollow;

    private void Start()
    {
        targetCamera = FindObjectsByType<Camera>(FindObjectsSortMode.None);
    }

    void Update()
    {
        currentCamFollow = targetCamera[0].transform;

        if (Vector3.Distance(transform.position, currentCamFollow.transform.position) >
            Vector3.Distance(transform.position, targetCamera[1].transform.position))
        {
            currentCamFollow = targetCamera[1].transform;
        }


        var _rotation = currentCamFollow.transform.rotation;
            
        transform.LookAt(transform.position + _rotation * Vector3.forward, _rotation * Vector3.up);
    }
}
