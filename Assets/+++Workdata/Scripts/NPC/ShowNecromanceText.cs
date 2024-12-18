using TMPro;
using UnityEngine;

public class ShowNecromanceText : MonoBehaviour
{
    public Canvas[] NecromanceTextCanvas;
    [SerializeField] private LayerMask playerLayers;
    [SerializeField] private float detectPlayerRadius;
    [HideInInspector] public bool wholeHordeDead;

    private void Start()
    {
        NecromanceTextCanvas = GetComponentsInChildren<Canvas>();
    }

    private void Update()
    {
        CheckPlayerInRange();
    }

    private void CheckPlayerInRange()
    {
        if(!wholeHordeDead)
        {
            return;
        }

        bool isPlayerNearby = Physics2D.OverlapCircleAll(transform.position, detectPlayerRadius, playerLayers).Length > 0;

        foreach (var canvas in NecromanceTextCanvas)
        {
            canvas.transform.GetChild(0).gameObject.SetActive(isPlayerNearby);
        }
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
