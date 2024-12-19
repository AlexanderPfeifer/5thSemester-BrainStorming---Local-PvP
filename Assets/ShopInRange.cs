using UnityEngine;

public class ShopInRange : MonoBehaviour
{
    [SerializeField] private CanvasGroup shopCanvas;
    [SerializeField] private LayerMask playerLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((playerLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            shopCanvas.alpha = 1;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((playerLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            shopCanvas.alpha = 0;
        }
    }
}
