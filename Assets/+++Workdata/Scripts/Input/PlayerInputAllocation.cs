using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputAllocation : MonoBehaviour
{
    private PlayerInput playerInput;

    private ZombiePlayerHordeRegistry zombiePlayerHordeRegistry;
    private NecromanceHorde necromanceHorde;

    private bool canTakeInput;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        zombiePlayerHordeRegistry = FindObjectsByType<ZombiePlayerHordeRegistry>(FindObjectsSortMode.None).FirstOrDefault(aZM => aZM.GetPlayerIndex() == playerInput.playerIndex);

        necromanceHorde = FindObjectsByType<ZombiePlayerHordeRegistry>(FindObjectsSortMode.None).FirstOrDefault(aZM => aZM.GetPlayerIndex() == playerInput.playerIndex).necromanceHorde;

        necromanceHorde.GetComponent<ZombiePlayerHordeRegistry>().SpawnPlayerZombie();

        canTakeInput = true;   
    }

    public void OnMove(InputValue inputValue)
    {
        if(!canTakeInput) 
            return;

        zombiePlayerHordeRegistry.mainZombie.transform.GetComponent<PlayerMovement>().OnMove(inputValue);
    }

    public void OnNecromance()
    {
        if (!canTakeInput)
            return;

        necromanceHorde.OnNecromance();
    }
}
