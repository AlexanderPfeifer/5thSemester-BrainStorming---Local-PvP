using UnityEngine;

public class ShowNecromanceText : MonoBehaviour
{
    [HideInInspector] public GameObject NecromanceText;
    [SerializeField] private LayerMask playerLayers;
    [SerializeField] private float detectPlayerRadius;
    [HideInInspector] public bool wholeHordeDead;

    private void Start()
    {
        NecromanceText = GetComponentInChildren<Canvas>().transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        CheckPlayerInRange();
    }

    private void CheckPlayerInRange()
    {
        if(!wholeHordeDead)
            return;

        var playerHit = Physics2D.OverlapCircleAll(transform.position, detectPlayerRadius, playerLayers);

        NecromanceText.SetActive(playerHit.Length > 0);
    }
}
