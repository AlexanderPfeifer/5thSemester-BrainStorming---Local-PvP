using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInputAllocation : MonoBehaviour
{
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public void OnMove(InputValue inputValue)
    {
        var allZombieManager = FindObjectsByType<ZombieManager>(FindObjectsSortMode.None).FirstOrDefault(aZM => aZM.GetPlayerIndex() == playerInput.playerIndex);

        foreach (var zombieMovement in allZombieManager.Zombies)
        {
            zombieMovement.GetComponent<ZombieMovement>().OnMove(inputValue);
        }
    }

    public void OnNecromance()
    {

    }
}
