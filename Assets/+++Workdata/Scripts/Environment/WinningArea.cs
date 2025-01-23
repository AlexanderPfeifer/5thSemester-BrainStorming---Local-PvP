using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinningArea : MonoBehaviour
{
    [SerializeField] private Slider player1PointsSlider;
    [SerializeField] private Slider player2PointsSlider;

    [SerializeField] private ParticleSystem player1Particles;
    [SerializeField] private ParticleSystem player2Particles;

    [SerializeField] private NecromanceHorde player1NecromanceHorde;

    private int player1Points;
    private int player2Points;

    [SerializeField] private int pointsToWin;

    public bool canObtainPoints;

    [SerializeField] public float zombiesInRangeRadius;

    [SerializeField] private ParticleSystem brainActiveParticles;
    [SerializeField] private ParticleSystem brainObtainPointsParticle;

    [HideInInspector] public Image obtainPointsImage;

    [HideInInspector] public CanvasGroup canvasGroup;
    
    private void Start()
    {
        canObtainPoints = false;
        brainActiveParticles = GetComponentInChildren<ParticleSystem>();
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        obtainPointsImage = canvasGroup.GetComponentInChildren<Image>();
    }

    private void Update()
    {
        if (canObtainPoints && !brainActiveParticles.isPlaying)
        {
            brainActiveParticles.Play();
        }
        else if(!canObtainPoints && brainActiveParticles.isPlaying)
        {
            brainActiveParticles.Stop();
        }
    }

    public void PlayerPointsAllocation(List<Collider> playerZombies, NecromanceHorde necromanceHorde)
    {
        var _brainActiveParticles = brainObtainPointsParticle.main;
        var _player1Particles = player1Particles.main;
        var _player2Particles = player2Particles.main;

        List<Collider> _playerCollider = new (playerZombies);

        if (playerZombies.Count > 1)
        {
            foreach (var _player1Zombie in playerZombies)
            {
                if (_player1Zombie == null) // Check if the Transform is destroyed
                {
                    _playerCollider.Remove(_player1Zombie);
                    Debug.LogWarning("A destroyed Transform was found in the _player1Zombie.");
                    continue; // Skip this entry
                }
                
                if (_player1Zombie.TryGetComponent(out NPCMovement _npcMovement))
                {
                    var _obtainPointsParticles = _npcMovement.ObtainPointsParticles.main;
                    _obtainPointsParticles.startColor = necromanceHorde == player1NecromanceHorde ? _player1Particles.startColor : _player2Particles.startColor;
                    _npcMovement.ObtainPointsParticles.Play();
                }
            }

            necromanceHorde.zombiesNearBrainPlayer.Clear();
            necromanceHorde.zombiesNearBrainPlayer = _playerCollider;
            playerZombies = _playerCollider;

            if (necromanceHorde == player1NecromanceHorde)
            {
                foreach (var _winningArea in FindAnyObjectByType<ShowDirectionOfWinArea>().winningArea)
                {
                    _winningArea.player1Points += playerZombies.Count;
                }
                
                player1PointsSlider.value = (float)player1Points / pointsToWin;
                
                _brainActiveParticles.startColor = _player1Particles.startColor;
                
                if (player1Points >= pointsToWin)
                {
                    MenuUI.Instance.ShowWin("PLAYER 1 WINS THIS ROUND!");
                }
            }
            else
            {
                foreach (var _winningArea in FindAnyObjectByType<ShowDirectionOfWinArea>().winningArea)
                {
                    _winningArea.player2Points += playerZombies.Count;
                }
                
                player2PointsSlider.value = (float)player2Points / pointsToWin;
                
                _brainActiveParticles.startColor = _player2Particles.startColor;
                
                if (player2Points >= pointsToWin)
                {
                    MenuUI.Instance.ShowWin("PLAYER 2 WINS THIS ROUND!");
                } 
            }
            
            brainObtainPointsParticle.Play();
            
            AudioManager.Instance.PlayWithRandomPitch("ObtainingPoints");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, zombiesInRangeRadius);   
    }
}