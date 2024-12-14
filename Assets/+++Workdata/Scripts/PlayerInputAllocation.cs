using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInputAllocation : MonoBehaviour
{
    private PlayerInput playerInput;
    private List<ZombieMovement> zombieMovementList = new List<ZombieMovement>();

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        var allZombieManager = FindObjectsByType<ZombieManager>(FindObjectsSortMode.None).FirstOrDefault(aZM => aZM.GetPlayerIndex() == playerInput.playerIndex);
        foreach (var zombie in allZombieManager.Zombies)
        {
            zombieMovementList.Add(zombie.GetComponent<ZombieMovement>());
        }
    }

    public void OnMove(InputValue inputValue)
    {
        foreach (var zombieMovement in zombieMovementList)
        {
            zombieMovement.OnMove(inputValue);
        }
    }

    public void OnNecromance()
    {

    }
}
