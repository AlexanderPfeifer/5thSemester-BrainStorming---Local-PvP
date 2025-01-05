using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MenuUI : MonoBehaviour
{
    //its been a while since I had to code, dont mind me. feel free to optimize

    [SerializeField] private AudioMixer volumeMixer;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public void Start()
    {
        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("InGame");
    }

    public void CloseGame()
    {
        Application.Quit();
        Debug.Log("game quit");
    }

    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        volumeMixer.SetFloat("Master", Mathf.Log10(volume)*20);
    }
    
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        volumeMixer.SetFloat("Music", Mathf.Log10(volume)*20);
    }
    
    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        volumeMixer.SetFloat("SFX", Mathf.Log10(volume)*20);
    }
}
