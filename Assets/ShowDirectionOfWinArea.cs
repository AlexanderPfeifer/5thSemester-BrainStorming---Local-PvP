using UnityEngine;

public class ShowDirectionOfWinArea : MonoBehaviour
{
    [SerializeField] private RectTransform player1Arrow;
    [SerializeField] private RectTransform player2Arrow;
    
    public Transform player1;
    public Transform player2;

    [SerializeField] private Transform winningArea;

    public bool showDirection;

    private void Update()
    {
        if(!showDirection)
            return;
        
        RotateArrowToWinningArea(player1, player1Arrow);
        
        RotateArrowToWinningArea(player2, player2Arrow);
    }

    private void RotateArrowToWinningArea(Transform player, RectTransform playerArrow)
    {
        Vector3 _winningArea = winningArea.position;
        _winningArea.y = 0f;

        Vector3 _player = player.transform.position;
        _winningArea.x -= _player.x;
        _winningArea.z -= _player.z;

        float _angle = -Mathf.Atan2(_winningArea.x, _winningArea.z) * Mathf.Rad2Deg;
        playerArrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, _angle));
    }
}
