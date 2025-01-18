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

    public void PlayerPointsAllocation(Collider[] player1Zombies, Collider[] player2Zombies)
    {
        if(!brainActiveParticles.isPlaying)
            brainActiveParticles.Play();

        AudioManager.Instance.PlayWithRandomPitch("ObtainingPoints");


        if (player1Zombies.Length > 0)
        {
            foreach (var _player1Zombie in player1Zombies)
            {
                if(_player1Zombie.TryGetComponent(out NPCMovement _npcMovement))
                    _npcMovement.ObtainPointsParticles.Play();
            }
            player1Points += player1Zombies.Length;
            player1PointsSlider.value = (float)player1Points / pointsToWin;

            if (player1Points >= pointsToWin)
            {
                winScreen.SetActive(true);
            }
        }
        else
        {
            foreach (var _player2Zombie in player2Zombies)
            {
                if(_player2Zombie.TryGetComponent(out NPCMovement _npcMovement))
                    _npcMovement.ObtainPointsParticles.Play();
            }
            player2Points += player2Zombies.Length;
            player2PointsSlider.value = (float)player2Points / pointsToWin;

            if (player1Points >= pointsToWin)
            {
                winScreen.SetActive(true);
            }    
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, zombiesInRangeRadius);   
    }
}