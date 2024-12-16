using UnityEngine;

public class HudUIManager : MonoBehaviour
{
    void Start()
    {
        PlayerRegistryManager.Instance.AllPlayersReady += StartGameCountdown;
    }

    private void StartGameCountdown()
    {
        //StartGame 3..2..1 countdown -> let players only move after that
    }

    private void OnDestroy()
    {
        PlayerRegistryManager.Instance.AllPlayersReady -= StartGameCountdown;
    }
}
