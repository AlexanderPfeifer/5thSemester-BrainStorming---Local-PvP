using UnityEngine;

public class ShowDirectionOfWinArea : MonoBehaviour
{
    [SerializeField] private RectTransform player1Arrow;
    [SerializeField] private RectTransform player2Arrow;
    
    public Transform player1;
    public Transform player2;

    [SerializeField] public WinningArea[] winningArea;

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
        Vector3 _winningAreaPosition = transform.position;
            
        foreach (var _winningArea in winningArea)
        {
            if (_winningArea.canObtainPoints)
            {
                _winningAreaPosition = _winningArea.transform.position;
                _winningAreaPosition.y = 0f;
                break;
            }
        }

        if (_winningAreaPosition == transform.position)
        {
            Debug.Log("THERE IS NO WINNING AREA ASSIGNED");
            return;
        }

        Vector3 _player = player.transform.position;
        _winningAreaPosition.x -= _player.x;
        _winningAreaPosition.z -= _player.z;

        float _angle = -Mathf.Atan2(_winningAreaPosition.x, _winningAreaPosition.z) * Mathf.Rad2Deg;
        playerArrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, _angle));
    }
}
