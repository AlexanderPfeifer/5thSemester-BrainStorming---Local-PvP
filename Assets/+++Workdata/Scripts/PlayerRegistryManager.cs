using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerRegistryManager : MonoBehaviour
{
    private List<PlayerRegistry> playerRegistry = new List<PlayerRegistry>();

    public static PlayerRegistryManager Instance;

    [SerializeField] private GameObject Player1ReadyText;
    [SerializeField] private GameObject Player2ReadyText;


    void Awake()
    {
        if(Instance == null)
        { 
            Instance = this; 
        }
        else
        {
            Debug.LogWarning("Trying to create another Instance of PlayerRegistryManager!");
        }
    }

    public void HandlePlayerJoin(PlayerInput playerInput)
    {
        Debug.Log("Player " + playerInput.playerIndex + " Joined!");
        //playerInput.transform.SetParent(transform);
        if (!playerRegistry.Any(player => player.PlayerIndex == playerInput.playerIndex))
        {
            playerRegistry.Add(new PlayerRegistry(playerInput));
        }

        if (Player1ReadyText.activeSelf)
        {
            Player1ReadyText.SetActive(false);
        }
        else
        {
            Player2ReadyText.SetActive(false);
        }

        AllPlayersJoined();
    }

    private void AllPlayersJoined()
    {
        if (playerRegistry.Count == GetComponent<PlayerInputManager>().maxPlayerCount)
        {
            //StartGame 3..2..1
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
