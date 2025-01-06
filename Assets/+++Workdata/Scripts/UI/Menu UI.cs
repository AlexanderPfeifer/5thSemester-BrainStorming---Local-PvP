using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;
using UnityEngine.Rendering.Universal;

public class MenuUI : MonoBehaviour
{
    //its been a while since I had to code, dont mind me. feel free to optimize

    [SerializeField] private AudioMixer volumeMixer;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private List<GameObject> buttonList;

    [SerializeField] private bool fullscreenEnabled;

    [SerializeField] private GameObject pausePanel;
    
    [SerializeField] private GameObject pauseSettingsPanel;
    [SerializeField] private GameObject pauseControlsPanel;

    [SerializeField] private GameObject winPanel;

    [SerializeField] private GameObject creditsPanel;
    
    public void Start()
    {
        pausePanel.SetActive(false);
        pauseSettingsPanel.SetActive(false);
        pauseControlsPanel.SetActive(false);
        winPanel.SetActive(false);
        creditsPanel.SetActive(false);
        
        
        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();

        fullscreenEnabled = false;
    }

    public void Update()
    {
        if (fullscreenEnabled)
        {
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreen = false;
        }

        if (winPanel.activeSelf == true)
        {
            pausePanel.SetActive(false);
            pauseSettingsPanel.SetActive(false);
            pauseControlsPanel.SetActive(false);
            creditsPanel.SetActive(false);
        }
    }

    #region MAINMENU

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

    public void DisableButtons()
    {
        Debug.Log("settings clicked");

        foreach (GameObject button in buttonList)
        {
            button.gameObject.GetComponent<Button>().enabled = false;
            button.gameObject.GetComponent<EventTrigger>().enabled = false;
            button.gameObject.GetComponent<Animator>().enabled = false;
        }
    }

    public void ResetButtons()
    {
        Debug.Log("reset buttons");

        foreach (GameObject button in buttonList)
        {
            button.gameObject.GetComponent<Button>().enabled = true;
            button.gameObject.GetComponent<EventTrigger>().enabled = true;
            button.gameObject.GetComponent<Animator>().enabled = true;
        }
    }

    public void FullScreenToggle()
    {
        fullscreenEnabled = !fullscreenEnabled;
    }
    
    #endregion

    #region PAUSE
    
    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }
    
    public void OpenSettings()
    {
        pauseSettingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        pauseSettingsPanel.SetActive(false);
    }
    
    public void OpenControls()
    {
        pauseControlsPanel.SetActive(true);
    }

    public void CloseControls()
    {
        pauseControlsPanel.SetActive(false);
    }

    public void BackToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    #endregion

    #region WIN/LOSE

    public void ShowWin()
    {
        Time.timeScale = 0;
        winPanel.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("InGame");
    }

    #endregion

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
    }
 
}
