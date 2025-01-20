using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class MenuUI : MonoBehaviour
{
    //its been a while since I had to code, dont mind me. feel free to optimize

    [SerializeField] private AudioMixer volumeMixer;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private List<GameObject> buttonList;

    [SerializeField] private bool fullscreenEnabled;
    [SerializeField] private GameObject fullscreenCrossed;
    
    [SerializeField] private GameObject pausePanel;
    
    [SerializeField] private GameObject audioSettingsPanel;
    [SerializeField] private GameObject visualSettingsPanel;
    [SerializeField] private GameObject settingsControlsPanel;
    [SerializeField] private GameObject pauseControlsPanel;

    [SerializeField] private GameObject winPanel;

    [SerializeField] private GameObject creditsPanel;

    [SerializeField] private Resolution[] resolutions;
    [SerializeField] private List<TextMeshProUGUI> resolutionsText;
    [SerializeField] private int currentResolutionIndex = 0;
    [SerializeField] private int currentResolutionsText = 0;
    
    public void Start()
    {
        pausePanel.SetActive(false);
        
        pauseControlsPanel.SetActive(false);
        winPanel.SetActive(false);
        
        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();

        fullscreenEnabled = false;
        fullscreenCrossed.SetActive(false);

        resolutions = Screen.resolutions;
        Screen.SetResolution(Mathf.RoundToInt(resolutions[0].width), Mathf.RoundToInt(resolutions[0].height), fullscreenEnabled);
        currentResolutionIndex = 0;
        
        foreach (TextMeshProUGUI resText in resolutionsText)
        {
            resText.gameObject.SetActive(false);
        }
        
        resolutionsText[0].gameObject.SetActive(true);
    }

    public void Update()
    {
        if (fullscreenEnabled)
        {
            Screen.fullScreen = true;
            fullscreenCrossed.SetActive(true);
            
        }
        else
        {
            Screen.fullScreen = false;
            fullscreenCrossed.SetActive(false);
        }

        if (winPanel.activeSelf)
        {
            pausePanel.SetActive(false);
            visualSettingsPanel.SetActive(false);
            pauseControlsPanel.SetActive(false);
            creditsPanel.SetActive(false);
        }
    }

    private void CheckActivePanel()
    {
        if (audioSettingsPanel.activeSelf)
        {
            
        }
        else if(visualSettingsPanel.activeSelf)
        {
            
        }
        else if (settingsControlsPanel.activeSelf)
        {
            
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
    
    public void SetSelectedButton(GameObject selectedObject)
    {
        var _eventSystem = FindAnyObjectByType<EventSystem>();

        _eventSystem.SetSelectedGameObject(null);
        _eventSystem.SetSelectedGameObject(selectedObject);

        if (selectedObject.TryGetComponent(out Button _button))
        {
            _button.GetComponent<Animator>().SetTrigger("Highlighted");
        }
    }

    public void DisableButtons()
    {
        foreach (GameObject button in buttonList)
        {
            button.gameObject.GetComponent<Button>().enabled = false;
            button.gameObject.GetComponent<EventTrigger>().enabled = false;
            button.gameObject.GetComponent<Animator>().enabled = false;
        }
    }

    public void ResetButtons()
    {
        foreach (GameObject button in buttonList)
        {
            button.gameObject.GetComponent<Button>().enabled = true;
            button.gameObject.GetComponent<EventTrigger>().enabled = true;
            button.gameObject.GetComponent<Animator>().enabled = true;
            
            button.gameObject.GetComponent<Animator>().Rebind();
            
            button.gameObject.GetComponent<Animator>().transform.GetChild(1).GetComponent<Animator>().Rebind();
        }
    }

    public void ChangeResolution()
    {
        currentResolutionIndex = (currentResolutionIndex + 1) % resolutions.Length;

        if (currentResolutionIndex > resolutions.Length)
        {
            currentResolutionIndex = 0;
        }
            
        Debug.Log(resolutions.Length);    
        
        Debug.Log(Screen.currentResolution);
        
        resolutionsText[currentResolutionsText].gameObject.SetActive(false);
        currentResolutionsText = (currentResolutionsText + 1) % resolutionsText.Count;
        resolutionsText[currentResolutionsText].gameObject.SetActive(true);
        
        if (currentResolutionsText > resolutionsText.Count)
        {
            currentResolutionsText = 0;
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
        audioSettingsPanel.SetActive(true);
        audioSettingsPanel.GetComponent<Animator>().SetTrigger("Pressed");
    }

    public void CloseSettings()
    {
        audioSettingsPanel.SetActive(false);
        audioSettingsPanel.GetComponent<Animator>().SetTrigger("Pressed");
    }
    
    public void OpenControls()
    {
        pauseControlsPanel.SetActive(true);
        pauseControlsPanel.GetComponent<Animator>().SetTrigger("Pressed");
    }

    public void CloseControls()
    {
        pauseControlsPanel.SetActive(false);
        pauseControlsPanel.GetComponent<Animator>().SetTrigger("Pressed");
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

    public void CloseCredits()
    {
        creditsPanel.GetComponentInChildren<Animator>().SetTrigger("Pressed");
        ResetButtons();
    }
}
