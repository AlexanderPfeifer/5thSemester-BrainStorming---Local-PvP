using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class NecromanceHorde : MonoBehaviour
{
    public HashSet<Transform> NecromancableZombieHorde = new();
    [HideInInspector] public Transform InteractableLever;
    [HideInInspector] public Transform InteractableBrain;
    [HideInInspector] public List<Collider> zombiesNearBrainPlayer1;
    [HideInInspector] public List<Collider> zombiesNearBrainPlayer2;
    public ParticleSystem interactCircle;


    [SerializeField] private string necromantedZombieName;

    [SerializeField] private VisualEffect bloodEffect;

    public Transform ParentObject;
    [SerializeField] private LayerMask attackableZombieLayer;
    [SerializeField] private LayerMask ownZombieLayer;
    [SerializeField] private Sprite zombieVisual;

    private void Update()
    {
        if(GetComponentInChildren<DetectInteractable>())
            interactCircle.transform.position = GetComponentInChildren<DetectInteractable>().transform.position;
    }

    public void OnNecromance()
    {
        TryNecromancing();

        TryInteractingWithLever();
        
        TryInteractingWithBrain();
    }

    void TryNecromancing()
    {
        var _necromancableHordeSet = new HashSet<Transform>();
        var _necromancableZombieSet = new HashSet<GameObject>();

        // Clear the sets to ensure they are reset each time the method is triggered
        _necromancableHordeSet.Clear();
        _necromancableZombieSet.Clear();
        
        foreach (Transform _zombieHorde in NecromancableZombieHorde)
        {
            _necromancableHordeSet.Add(_zombieHorde);

            foreach(Transform _horde in _necromancableHordeSet)
            {
                if (_horde == null) // Check if the Transform is destroyed
                {
                    Debug.LogWarning("A destroyed Transform was found in the horde.");
                    continue; // Skip this entry
                }
                
                foreach (Transform _zombie in _horde)
                {
                    if (_zombie == null) // Check if the Transform is destroyed
                    {
                        Debug.LogWarning("A destroyed Transform was found in the zombie.");
                        continue; // Skip this entry
                    }
                    
                    _necromancableZombieSet.Add(_zombie.gameObject);
                }
            }
        }

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

    void PlayParticleEffect()
    {
        if(!interactCircle.isPlaying)
            interactCircle.Play();
    }

    private void TryInteractingWithLever()
    {
        if (InteractableLever != null)
        {
            Image _leverImage = InteractableLever.GetComponentInChildren<Image>();
            _leverImage.fillAmount += .25f;
            
            if (_leverImage.fillAmount >= .9f)
            {
                InteractableLever.GetComponentInChildren<Lever>().PullLever();
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
                InteractableBrain.GetComponentInChildren<WinningArea>().PlayerPointsAllocation(zombiesNearBrainPlayer1, zombiesNearBrainPlayer2, this);

                _brainImage.fillAmount = 0f;
            }
        }
    }

    private void NecromanceZombie(GameObject necromancableZombie)
    {
        var _necromancableZombieCachedData = necromancableZombie.GetComponent<CachedZombieData>();

        Destroy(necromancableZombie.transform.parent.GetComponent<ShowNecromanceText>());
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
        _necromancableZombieCachedData.NPCMovement.MainZombieMovement = _necromancableZombieCachedData.ZombiePlayerHordeRegistry.mainZombie.GetComponent<PlayerMovement>();
        _necromancableZombieCachedData.NPCMovement.IsNecromanced = true;

        _necromancableZombieCachedData.MeshRenderer.material.mainTexture = zombieVisual.texture;
        
        _necromancableZombieCachedData.Health.ResetHealth();
        _necromancableZombieCachedData.Health.IsPlayerZombie = true;
        _necromancableZombieCachedData.Health.bloodEffect = bloodEffect;

        _necromancableZombieCachedData.Animator.enabled = true;
    }
}
