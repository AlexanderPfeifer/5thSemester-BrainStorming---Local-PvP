using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WinningArea : MonoBehaviour
{
    [Header("Points Slider")]
    [SerializeField] private Slider player1PointsSlider;
    [SerializeField] private Slider player2PointsSlider;

    [Header("Particles")]
    [SerializeField] private ParticleSystem player1Particles;
    [SerializeField] private ParticleSystem player2Particles;
    [FormerlySerializedAs("brainActiveParticles")] [SerializeField] private ParticleSystem brainMiniMapActiveParticles;
    [SerializeField] private ParticleSystem brainObtainPointsParticle;

    [Header("Player Check")]
    [SerializeField] private NecromanceHorde player1NecromanceHorde;

    [Header("Points")]
    [SerializeField] private int pointsToWin;
    private int player1Points;
    private int player2Points;
    [HideInInspector] public bool canObtainPoints;

    [Header("Zombie Detection")]
    [SerializeField] public float zombiesInRangeRadius;

    [Header("UI")]
    [FormerlySerializedAs("obtainPointsImage")] [HideInInspector] public Image interactImage;
    [HideInInspector] public CanvasGroup canvasGroup;
    
    private void Start()
    {
        canObtainPoints = false;
        brainMiniMapActiveParticles = GetComponentInChildren<ParticleSystem>();
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        interactImage = canvasGroup.GetComponentInChildren<Image>();
    }

    private void Update()
    {
        SetBrainMiniMapParticles();
    }

    private void SetBrainMiniMapParticles()
    {
        switch (canObtainPoints)
        {
            case true when !brainMiniMapActiveParticles.isPlaying:
                brainMiniMapActiveParticles.Play();
                break;
            case false when brainMiniMapActiveParticles.isPlaying:
                brainMiniMapActiveParticles.Stop();
                break;
        }
    }

    public void PlayerPointsAllocation(List<Collider> playerZombies, NecromanceHorde necromanceHorde)
    {
        var _brainActiveParticles = brainObtainPointsParticle.main;
        var _player1Particles = player1Particles.main;
        var _player2Particles = player2Particles.main;

        List<Collider> _removedDestroyedZombiesList = new (playerZombies);

        if (playerZombies.Count > 1)
        {
            foreach (var _playerZombie in playerZombies)
            {
                //Some zombies are always destroyed but I do not know why so I reassure nothing unintended happens
                if (_playerZombie == null) 
                {
                    _removedDestroyedZombiesList.Remove(_playerZombie);
                    Debug.LogWarning("A destroyed Transform was found in the _player1Zombie.");
                    continue; 
                }
                
                //Ask for NPCMovement so the main zombie does not play any VFX because it is always inside the zombie list
                if (_playerZombie.TryGetComponent(out NPCMovement _npcMovement))
                {
                    var _obtainPointsParticles = _npcMovement.ObtainPointsParticles.main;
                    _obtainPointsParticles.startColor = necromanceHorde == player1NecromanceHorde ? _player1Particles.startColor : _player2Particles.startColor;
                    _npcMovement.ObtainPointsParticles.Play();
                }
            }

            //Always clearing the zombie list because I assign the new list after checking for all the destroyed zombies and it would otherwise just add upon it
            necromanceHorde.zombiesNearBrainPlayer.Clear();
            necromanceHorde.zombiesNearBrainPlayer = _removedDestroyedZombiesList;
            playerZombies = _removedDestroyedZombiesList;

            if (necromanceHorde == player1NecromanceHorde)
            {
                foreach (var _winningArea in FindAnyObjectByType<WinningAreas>().winningArea)
                {
                    _winningArea.player1Points += playerZombies.Count;
                }
                
                player1PointsSlider.value = (float)player1Points / pointsToWin;
                
                _brainActiveParticles.startColor = _player1Particles.startColor;
                
                if (player1Points >= pointsToWin)
                {
                    MenuUI.Instance.ShowWin("PLAYER 1 WINS THIS ROUND!");
                    AudioManager.Instance.FadeOut("InGameMusic");
                    AudioManager.Instance.FadeIn("WinSound");
                }
            }
            else
            {
                foreach (var _winningArea in FindAnyObjectByType<WinningAreas>().winningArea)
                {
                    _winningArea.player2Points += playerZombies.Count;
                }
                
                player2PointsSlider.value = (float)player2Points / pointsToWin;
                
                _brainActiveParticles.startColor = _player2Particles.startColor;
                
                if (player2Points >= pointsToWin)
                {
                    MenuUI.Instance.ShowWin("PLAYER 2 WINS THIS ROUND!");
                    AudioManager.Instance.FadeOut("InGameMusic");
                    AudioManager.Instance.FadeIn("WinSound");
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