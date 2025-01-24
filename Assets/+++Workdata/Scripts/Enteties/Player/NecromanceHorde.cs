using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.VFX;

//This class should be called Interact, but I don't want to break things renaming it
public class NecromanceHorde : MonoBehaviour
{
    [Header("Necromance")]
    public readonly HashSet<Transform> NecromancableZombieHorde = new();
    [FormerlySerializedAs("zombiesNearBrainPlayer1")] [HideInInspector] public List<Collider> zombiesNearBrainPlayer;
    [SerializeField] private string necromantedZombieName;
    public Transform ParentObject;
    [SerializeField] private LayerMask ownZombieLayer;
    [SerializeField] private LayerMask attackableZombieLayer;
    [SerializeField] private Sprite zombieVisual;
    
    [Header("Lever")]
    [HideInInspector] public Transform InteractableLever;
    
    [Header("Brain")]
    [HideInInspector] public Transform InteractableBrain;

    [Header("VFX")]
    public ParticleSystem interactCircle;
    [SerializeField] private VisualEffect bloodEffect;

    private void Update()
    {
        if(GetComponentInChildren<DetectInteractable>())
            interactCircle.transform.position = GetComponentInChildren<DetectInteractable>().transform.position;
    }

    public void OnInteract()
    {
        TryNecromancing();

        TryInteractingWithLever();
        
        TryInteractingWithBrain();
    }

    private void TryInteractingWithLever()
    {
        if (InteractableLever != null)
        {
            Image _leverImage = InteractableLever.GetComponentInChildren<Image>();
            _leverImage.fillAmount += .25f;
            
            if (_leverImage.fillAmount >= .9f)
            {
                InteractableLever.GetComponentInChildren<Lever>().PullLever(true);
                var _mainModule = interactCircle.main;
                ParticleSystem.MinMaxCurve _initialSize = _mainModule.startSize;
                _mainModule.startSize = 20;
                PlayParticleEffect();
                _mainModule.startSize = _initialSize;
            }
        }
    }
    
    private void TryInteractingWithBrain()
    {
        if (InteractableBrain != null && InteractableBrain.GetComponentInChildren<CanvasGroup>().alpha >= 1)
        {
            Image _brainImage = InteractableBrain.GetComponentInChildren<Image>();
            _brainImage.fillAmount += 0.25f;

            if (_brainImage.fillAmount >= 0.9f)
            {
                InteractableBrain.GetComponentInChildren<WinningArea>().PlayerPointsAllocation(zombiesNearBrainPlayer, this);

                _brainImage.fillAmount = 0f;
            }
        }
    }

    void TryNecromancing()
    {
        var _necromancableHordeSet = new HashSet<Transform>();
        var _necromancableZombieSet = new HashSet<GameObject>();

        // Clear the sets because otherwise the hashset uses the sets I previously assigned
        _necromancableHordeSet.Clear();
        _necromancableZombieSet.Clear();
        
        foreach (Transform _zombieHorde in NecromancableZombieHorde)
        {
            _necromancableHordeSet.Add(_zombieHorde);

            foreach(Transform _horde in _necromancableHordeSet)
            {
                if (_horde == null)
                {
                    Debug.LogWarning("A destroyed Transform was found in the horde.");
                    continue;
                }
                
                foreach (Transform _zombie in _horde)
                {
                    if (_zombie == null) 
                    {
                        Debug.LogWarning("A destroyed Transform was found in the zombie.");
                        continue; 
                    }
                    
                    _necromancableZombieSet.Add(_zombie.gameObject);
                }
            }
        }

        //Clear the hordes because they all got assigned in hashsets and are in the process of being necromanced
        NecromancableZombieHorde.Clear();

        foreach(var _humans in _necromancableZombieSet)
        {
            Image _humanImage = _humans.GetComponentInChildren<Image>();

            _humanImage.fillAmount += .25f;
            if (_humanImage.fillAmount >= 1)
            {
                PlayParticleEffect();
                AudioManager.Instance.PlayWithRandomPitch("Necromance");
                NecromanceZombie(_humans);
                _humans.transform.parent = ParentObject;   
            }
        }
    }

    private void NecromanceZombie(GameObject necromancableZombie)
    {
        var _necromancableZombieCachedData = necromancableZombie.GetComponent<CachedZombieData>();

        Destroy(necromancableZombie.transform.parent.GetComponent<ShowNecromanceTriangle>());
        Destroy(_necromancableZombieCachedData.transform.GetComponentInChildren<Canvas>().gameObject);

        necromancableZombie.name = necromantedZombieName;

        _necromancableZombieCachedData.ZombiePlayerHordeRegistry = GetComponent<ZombiePlayerHordeRegistry>();

        _necromancableZombieCachedData.ZombiePlayerHordeRegistry.RegisterZombie(necromancableZombie);
        
        _necromancableZombieCachedData.AutoAttack.attackableZombieLayer = attackableZombieLayer;
        //Use this weird function, because if I just set the gameobject.layer equal to the ownZombieLayer, it throws an error trying to set 2 different layers
        _necromancableZombieCachedData.AutoAttack.gameObject.layer = Mathf.RoundToInt(Mathf.Log(ownZombieLayer.value, 2));
        _necromancableZombieCachedData.AutoAttack.ResetAttack();
        _necromancableZombieCachedData.AutoAttack.cachedZombieData.NecromanceHorde = this;
        _necromancableZombieCachedData.AutoAttack.enabled = true;

        //We can get the same zombie layer of this object because the zombie is going to join this zombies team
        _necromancableZombieCachedData.NPCMovement.MainZombieMovement = _necromancableZombieCachedData.ZombiePlayerHordeRegistry.MainZombie.GetComponent<PlayerMovement>();
        _necromancableZombieCachedData.NPCMovement.IsZombie = true;

        _necromancableZombieCachedData.MeshRenderer.material.mainTexture = zombieVisual.texture;
        
        _necromancableZombieCachedData.Health.ResetHealth();
        _necromancableZombieCachedData.Health.IsPlayerZombie = true;
        _necromancableZombieCachedData.Health.bloodEffect = bloodEffect;

        _necromancableZombieCachedData.Animator.enabled = true;
    }
    
    void PlayParticleEffect()
    {
        if(!interactCircle.isPlaying)
            interactCircle.Play();
    }
}
