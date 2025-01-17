using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NecromanceHorde : MonoBehaviour
{
    public HashSet<Transform> NecromancableZombieHorde = new();
    [HideInInspector] public Transform InteractableLever;
    [HideInInspector] public Transform InteractableBrain;
    [HideInInspector] public int zombiesNearBrainPlayer1;
    [HideInInspector] public int zombiesNearBrainPlayer2;
    [SerializeField] private ParticleSystem interactCircle;


    [SerializeField] private string necromantedZombieName;

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
                foreach (Transform _zombie in _horde)
                {
                    _necromancableZombieSet.Add(_zombie.gameObject);
                }
            }
        }

        NecromancableZombieHorde.Clear();

        foreach(var _zombie in _necromancableZombieSet)
        {
            _zombie.GetComponentInChildren<Image>().fillAmount += .25f;
            if (_zombie.GetComponentInChildren<Image>().fillAmount >= 1)
            {
                PlayParticleEffect();
                AudioManager.Instance.PlayWithRandomPitch("Necromance");
                NecromanceZombie(_zombie);
                _zombie.transform.parent = ParentObject;   
            }
        }
        
        TryInteractingWithLever();
        
        TryInteractingWithBrain();
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
            InteractableLever.GetComponentInChildren<Image>().fillAmount += .25f;
            
            if (InteractableBrain.GetComponentInChildren<Image>().fillAmount >= 1)
            {
                InteractableLever.GetComponentInChildren<Lever>().PullLever();
                PlayParticleEffect();
            }
        }
    }
    
    private void TryInteractingWithBrain()
    {
        if (InteractableBrain != null)
        {
            InteractableBrain.GetComponentInChildren<Image>().fillAmount += .25f;

            if (InteractableBrain.GetComponentInChildren<Image>().fillAmount >= 1)
            {
                InteractableBrain.GetComponentInChildren<WinningArea>().PlayerPointsAllocation(zombiesNearBrainPlayer1, zombiesNearBrainPlayer2);
                PlayParticleEffect();
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
        _necromancableZombieCachedData.AutoAttack.enabled = true;

        //We can get the same zombie layer of this object because the zombie is going to join this zombies team
        _necromancableZombieCachedData.NPCMovement.MainZombieMovement = _necromancableZombieCachedData.ZombiePlayerHordeRegistry.mainZombie.GetComponent<PlayerMovement>();
        _necromancableZombieCachedData.NPCMovement.IsNecromanced = true;

        _necromancableZombieCachedData.MeshRenderer.material.mainTexture = zombieVisual.texture;
        
        _necromancableZombieCachedData.Health.ResetHealth();
        _necromancableZombieCachedData.Health.IsPlayerZombie = true;

        _necromancableZombieCachedData.Animator.enabled = true;
    }
}
