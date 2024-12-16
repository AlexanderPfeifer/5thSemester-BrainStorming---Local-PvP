using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;

public class PlayerRegistryManager : MonoBehaviour
{
    private List<PlayerRegistry> playerRegistry = new List<PlayerRegistry>();

    public static PlayerRegistryManager Instance;

    [SerializeField] private GameObject[] playerReadyTexts;

    private Dictionary<int, GameObject> playerReadyTextMap;

    public event Action AllPlayersReady;

    private PlayerInputManager playerInputManager;


    void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();



        // Initialize the mapping based on player indices
        playerReadyTextMap = playerReadyTexts
            .Select((text, index) => new { Index = index, Text = text })
            .ToDictionary(entry => entry.Index, entry => entry.Text);



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
    }

    public void HandlePlayerJoin(PlayerInput playerInput)
    {
        Debug.Log("Player " + playerInput.playerIndex + " Joined!");



        if (playerRegistry.Any(player => player.PlayerIndex == playerInput.playerIndex))
        {
            Debug.LogWarning($"Player {playerInput.playerIndex} is already registered.");
            return;
        }

        if (!playerReadyTextMap.ContainsKey(playerInput.playerIndex))
        {
            Debug.LogError($"No ReadyText mapped for Player {playerInput.playerIndex}");
            return;
        }



        playerRegistry.Add(new PlayerRegistry(playerInput));

        playerReadyTextMap[playerInput.playerIndex].SetActive(false);

        if (playerRegistry.Count == playerInputManager.maxPlayerCount)
        {
            AllPlayersReady?.Invoke();
        }
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
