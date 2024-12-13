using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnPlayerOnReady : MonoBehaviour
{
    private int playerIndex;
    [SerializeField] private GameObject zombie;
    [SerializeField] private PlayerInput playerInput;

    private void Awake()
    {
        Instantiate(zombie);
    }

    public void SetPlayerIndex(int index)
    {
        playerIndex = index;
    }
}
