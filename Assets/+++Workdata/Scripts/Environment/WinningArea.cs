using UnityEngine;
using UnityEngine.UI;

public class WinningArea : MonoBehaviour
{
    [SerializeField] private Slider player1PointsSlider;
    [SerializeField] private Slider player2PointsSlider;

    private int player1Points;
    private int player2Points;

    [SerializeField] private int pointsToWin;
    [SerializeField] private GameObject winScreen;

    public bool canObtainPoints;

    [SerializeField] public float zombiesInRangeRadius;

    [SerializeField] private ParticleSystem brainActiveParticles;

    [SerializeField] public Image obtainPointsImage;

    [SerializeField] public CanvasGroup canvasGroup;

    private void Update()
    {
        if (canObtainPoints)
        {
            if(brainActiveParticles.isPlaying)
                brainActiveParticles.Stop();
        }
    }

    public void PlayerPointsAllocation(int player1Zombies, int player2Zombies)
    {
        if (obtainPointsImage.fillAmount >= 1)
        {
            if(!brainActiveParticles.isPlaying)
                brainActiveParticles.Play();

            AudioManager.Instance.PlayWithRandomPitch("ObtainingPoints");


            if (player1Zombies > 0)
            {
                player1Points += player1Zombies;
                player1PointsSlider.value = (float)player1Points / pointsToWin;

                if (player1Points >= pointsToWin)
                {
                    winScreen.SetActive(true);
                }
            }
            else
            {
                player2Points += player2Zombies;
                player2PointsSlider.value = (float)player2Points / pointsToWin;

                if (player1Points >= pointsToWin)
                {
                    winScreen.SetActive(true);
                }    
            }

            obtainPointsImage.fillAmount = 0;
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, zombiesInRangeRadius);   
    }
}