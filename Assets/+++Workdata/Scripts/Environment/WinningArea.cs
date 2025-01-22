using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinningArea : MonoBehaviour
{
    [SerializeField] private Slider player1PointsSlider;
    [SerializeField] private Slider player2PointsSlider;

    [SerializeField] private ParticleSystem player1Particles;
    [SerializeField] private ParticleSystem player2Particles;

    private int player1Points;
    private int player2Points;

    [SerializeField] private int pointsToWin;

    [HideInInspector] public bool canObtainPoints;

    [SerializeField] public float zombiesInRangeRadius;

    private ParticleSystem brainActiveParticles;

    [HideInInspector] public Image obtainPointsImage;

    [HideInInspector] public CanvasGroup canvasGroup;

    [SerializeField] private PointsVisualization player1PointsVisualization;
    [SerializeField] private PointsVisualization player2PointsVisualization;

    private void Start()
    {
        brainActiveParticles = GetComponentInChildren<ParticleSystem>();
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        obtainPointsImage = canvasGroup.GetComponentInChildren<Image>();
    }

    public void PlayerPointsAllocation(List<Collider> player1Zombies, List<Collider> player2Zombies)
    {
        var _brainActiveParticles = brainActiveParticles.main;
        var _player1Particles = player1Particles.main;
        var _player2Particles = player2Particles.main;
            
        if (player1Zombies.Count > 1)
        {
            foreach (var _player1Zombie in player1Zombies)
            {
                if (_player1Zombie == null) // Check if the Transform is destroyed
                {
                    Debug.LogWarning("A destroyed Transform was found in the _player1Zombie.");
                    continue; // Skip this entry
                }
                
                if (_player1Zombie.TryGetComponent(out NPCMovement _npcMovement))
                {
                    var _obtainPointsParticles = _npcMovement.ObtainPointsParticles.main;
                    _obtainPointsParticles.startColor = _player1Particles.startColor;
                    _npcMovement.ObtainPointsParticles.Play();
                }
            }

            StartCoroutine(player1PointsVisualization.ShowPlusIcon());
            
            player1Points += player1Zombies.Count;
            player1PointsSlider.value = (float)player1Points / pointsToWin;

            if (player1Points >= pointsToWin)
            {
                MenuUI.Instance.ShowWin("PLAYER 1 WINS THIS ROUND!");
            }
            
            _brainActiveParticles.startColor = _player1Particles.startColor;
        }
        
        if(player2Zombies.Count > 2)
        {
            foreach (var _player2Zombie in player2Zombies)
            {
                if (_player2Zombie == null) // Check if the Transform is destroyed
                {
                    Debug.LogWarning("A destroyed Transform was found in the _player2Zombie.");
                    continue; // Skip this entry
                }

                if (_player2Zombie.TryGetComponent(out NPCMovement _npcMovement))
                {
                    var _obtainPointsParticles = _npcMovement.ObtainPointsParticles.main;
                    _obtainPointsParticles.startColor = _player2Particles.startColor;
                    _npcMovement.ObtainPointsParticles.Play();   
                }
            }
            player2Points += player2Zombies.Count;
            player2PointsSlider.value = (float)player2Points / pointsToWin;
            
            StartCoroutine(player2PointsVisualization.ShowPlusIcon());

            if (player2Points >= pointsToWin)
            {
                MenuUI.Instance.ShowWin("PLAYER 2 WINS THIS ROUND!");
            }    
            
            _brainActiveParticles.startColor = _player2Particles.startColor;
        }
        
        if(!brainActiveParticles.isPlaying)
            brainActiveParticles.Play();

        AudioManager.Instance.PlayWithRandomPitch("ObtainingPoints");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, zombiesInRangeRadius);   
    }
}