using UnityEngine;

public class StartMainMenuSound : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Instance.FadeIn("MainMenuMusic");
    }
}
