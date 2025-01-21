using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;
using System.Collections;

public class PlayerRegistryManager : MonoBehaviour
{
    private List<PlayerRegistry> playerRegistry = new List<PlayerRegistry>();

    public static PlayerRegistryManager Instance;
    
    public event Action AllPlayersReady;

    private PlayerInputManager playerInputManager;
    

    void Awake()
    {
        //Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Trying to create another Instance of PlayerRegistryManager!");
            Destroy(gameObject);
        }



        playerInputManager = GetComponent<PlayerInputManager>();
    }

    public void HandlePlayerJoin(PlayerInput playerInput)
    {
        Debug.Log("Player " + playerInput.playerIndex + " Joined!");



        if (playerRegistry.Any(player => player.PlayerIndex == playerInput.playerIndex))
        {
            Debug.LogWarning($"Player {playerInput.playerIndex} is already registered.");
            return;
        }



        playerRegistry.Add(new PlayerRegistry(playerInput));
        
        if (playerRegistry.Count == playerInputManager.maxPlayerCount)
        {
            StartCoroutine(DelayedAllPlayersReady());
        }
    }

    private IEnumerator DelayedAllPlayersReady()
    {
        //delay it to give the second playerMovement some time to subscribe to this event on OnEnable
        yield return new WaitForSeconds(.5f);
        AllPlayersReady?.Invoke();
    }
}

    public class PlayerRegistry
    {
        public PlayerRegistry(PlayerInput playerInput)
        {
            PlayerIndex = playerInput.playerIndex;
            PlayerInput = playerInput;
        }
        public PlayerInput PlayerInput { get; set; }
        public int PlayerIndex { get; set; }
    }
    
