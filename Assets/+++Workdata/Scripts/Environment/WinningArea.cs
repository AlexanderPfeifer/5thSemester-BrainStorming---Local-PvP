using UnityEngine;
using UnityEngine.UI;

public class WinningArea : MonoBehaviour
{
    [SerializeField] private LayerMask player1Layer;
    [SerializeField] private LayerMask player2Layer;

    [SerializeField] private Slider player1PointsSlider;
    [SerializeField] private Slider player2PointsSlider;

    private int player1Points;
    private int player2Points;

    [SerializeField] private int pointsToWin;

    private float currentTimeToGetPoints;
    [SerializeField] private float maxTimeToGetPoints;
    [SerializeField] private GameObject winScreen;

    public bool canObtainPoints;


    private void Start()
    {
        currentTimeToGetPoints = maxTimeToGetPoints;
    }

    private void Update()
    {
        if(!canObtainPoints)
            return;
        
        PlayerPointsAllocation();
    }

    private void PlayerPointsAllocation()
    {
        var player1Zombies = Physics.OverlapSphere(transform.position, transform.localScale.x, player1Layer);

        var player2Zombies = Physics.OverlapSphere(transform.position, transform.localScale.x, player2Layer);

        //Check if there are more than 1 zombie to get points because the first one could be only the main zombie, which does not count
        if (player1Zombies.Length > 1 && player2Zombies.Length <= 1)
        {
            currentTimeToGetPoints -= Time.deltaTime;
            if (currentTimeToGetPoints < 0)
            {
                AudioManager.Instance.Play("ObtainingPoints", true);
                player1Points += player1Zombies.Length;
                player1PointsSlider.value = (float)player1Points / pointsToWin;

                if (player1Points >= pointsToWin)
                {
                    winScreen.SetActive(true);
                }

                currentTimeToGetPoints = maxTimeToGetPoints;
            }
        }
        else if (player2Zombies.Length > 1 && player1Zombies.Length <= 1)
        {
            currentTimeToGetPoints -= Time.deltaTime;
            if (currentTimeToGetPoints < 0)
            {
                AudioManager.Instance.Play("ObtainingPoints", true);
                player2Points += player2Zombies.Length;
                player2PointsSlider.value = (float)player2Points / pointsToWin;

                if (player1Points >= pointsToWin)
                {
                    winScreen.SetActive(true);
                }
                
                currentTimeToGetPoints = maxTimeToGetPoints;
            }
        }
        else
        {
            currentTimeToGetPoints = maxTimeToGetPoints;
        }
    }
}