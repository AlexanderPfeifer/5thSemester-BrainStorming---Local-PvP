using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputAllocation : MonoBehaviour
{
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public void OnMove(InputValue inputValue)
    {
        foreach (GameObject zombieMovement in GetAllZombiesByPlayerIndex())
        {
            zombieMovement.GetComponent<ZombieMovement>().OnMove(inputValue);
        }
    }

    public void OnNecromance()
    {
        foreach (GameObject zombieNecromance in new List<GameObject>(GetAllZombiesByPlayerIndex()))
        {
            zombieNecromance.GetComponent<Necromance>().OnNecromance();
        }
    }

    List<GameObject> GetAllZombiesByPlayerIndex()
    {
        return FindObjectsByType<ZombiePlayerHordeRegistry>(FindObjectsSortMode.None).FirstOrDefault(aZM => aZM.GetPlayerIndex() == playerInput.playerIndex).Zombies;
    }
}
