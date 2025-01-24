using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private AudioMixer volumeMixer;

    [Header("Sound Slider")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Main Menu")] 
    [SerializeField] private List<GameObject> buttonList;
    [SerializeField] private GameObject creditsPanel;

    [Header("Visual Settings")]
    private bool fullscreenEnabled = true;
    [SerializeField] private GameObject fullscreenCrossed;
    private int currentResolutionIndex;
    [SerializeField] private TextMeshProUGUI currentResolutionsText;

    [Header("Settings Panels")]
    [FormerlySerializedAs("audioSettingsPanel")] [SerializeField] private GameObject soundSettingsPanel;
    [SerializeField] private GameObject visualSettingsPanel;
    [FormerlySerializedAs("settingsControlsPanel")] [SerializeField] private GameObject controlsSettingsPanel;
    private float coolDown = 1f;
    [SerializeField] private GameObject firstSelectedSoundSettingsButton;
    [SerializeField] private GameObject firstSelectedVisualsSettingsButton;
    [SerializeField] private GameObject firstSelectedControlsSettingsButton;

    [Header("InGame Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private GameObject firstSelectedWinScreenButton;
    [SerializeField] private GameObject soundPauseSettingsPanel;
    [SerializeField] private GameObject visualPauseSettingsPanel;
    [SerializeField] private GameObject controlsPauseSettingsPanel;
    [SerializeField] private GameObject firstSelectedPauseSoundSettingsButton;
    [SerializeField] private GameObject firstSelectedPauseVisualsSettingsButton;
    [SerializeField] private GameObject firstSelectedPauseControlsSettingsButton;
    [SerializeField] private GameObject firstSelectedPauseButton;

    public static MenuUI Instance;


    private Resolution[] resolutions = {
        new() { width = 1024, height = 768 },
        new() { width = 1280, height = 1024 },
        new() { width = 1366, height = 768 },
        new() { width = 1920, height = 1080 },
        new() { width = 2560, height = 1440 },
        new() { width = 3840, height = 2160 }
    };

    [SerializeField] private int selectedResolutionIndex;

    [SerializeField] private Animator transitions;
        
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        
        SetResolution(3);
        currentResolutionIndex = 0;
    }
    
    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        pausePanel.SetActive(false);
        winPanel.SetActive(false);

        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();
        
        fullscreenCrossed.SetActive(false);

        ChangeResolutionText();
    }

    public void Update()
    {
        coolDown -= Time.unscaledDeltaTime;

        if (Input.GetKeyDown(KeyCode.JoystickButton4) || Input.GetKeyDown(KeyCode.Q))
        {
            SwitchToLeftPanel();
        }
        
        if (Input.GetKeyDown(KeyCode.JoystickButton5) || Input.GetKeyDown(KeyCode.E))
        {
            SwitchToRightPanel();
        }
        
        if (Input.GetKeyDown(KeyCode.JoystickButton9) || Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.Escape))
        {
            OnPause();
        }
    }

    #region MAINMENU

    public void StartGame()
    {
        AudioManager.Instance.FadeOut("MainMenuMusic");
        StartCoroutine("StartGameTransition");
    }

    private IEnumerator StartGameTransition()
    {
        transitions.SetTrigger("Start Crossfade");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("InGame");
    }

    public void CloseGame()
    {
        Application.Quit();
    }
    
    public void CloseCredits()
    {
        creditsPanel.GetComponentInChildren<Animator>().SetTrigger("Pressed");
        ResetButtons();
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

        if (selectedObject.TryGetComponent(out Button _button) && _button.TryGetComponent(out Animator _animator))
        {
            _animator.SetTrigger("Highlighted");
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
    void SetResolution(int index)
    {
        if (index >= 0 && index < resolutions.Length)
        {
            Resolution _res = resolutions[index];
            Screen.SetResolution(_res.width, _res.height, Screen.fullScreen);
        }
    }

    public void ChangeResolution(int selectedResolutionIndex)
    {
        currentResolutionIndex++;
        if (currentResolutionIndex >= resolutions.Length)
        {
            currentResolutionIndex = 0; 
        }
        
        SetResolution(currentResolutionIndex);
    }
    
    public void ChangeResolutionText()
    {
        if (currentResolutionIndex > resolutions.Length)
        { 
            currentResolutionIndex = 0;
        }
        
        currentResolutionsText.text = resolutions[currentResolutionIndex].width + " x "+ resolutions[currentResolutionIndex].height;
    }

    public void FullScreenToggle()
    {
        if (fullscreenEnabled)
        {
            fullscreenEnabled = false;
            Screen.fullScreen = false;
            fullscreenCrossed.SetActive(true);
        }
        else
        {
            fullscreenEnabled = true;
            Screen.fullScreen = true;
            fullscreenCrossed.SetActive(false);
        }
    }

    #endregion
    
    bool IsPlaying(Animator animator, string animationName)
    {
        if (!animator) 
            return false;

        AnimatorStateInfo _stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        return _stateInfo.IsName(animationName) && _stateInfo.normalizedTime < 1.0f;
    }
    
    bool IsInsideAnimation(Animator animator, string animationName)
    {
        if (!animator) 
            return false;

        AnimatorStateInfo _stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Check if the animation name matches and it is playing
        return _stateInfo.IsName(animationName);
    }
    
    public void SwitchToLeftPanel()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainMenu"))
        {
            SetPanelsToLeftInSettings(soundSettingsPanel, visualSettingsPanel, controlsSettingsPanel, firstSelectedSoundSettingsButton, firstSelectedVisualsSettingsButton);
        }
        else
        {
            SetPanelsToLeftInSettings(soundPauseSettingsPanel, visualPauseSettingsPanel, controlsPauseSettingsPanel, firstSelectedPauseSoundSettingsButton, firstSelectedPauseVisualsSettingsButton);
        }
    }
    
    public void SwitchToRightPanel()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainMenu"))
        {
            SetPanelsToRightInSettings(soundSettingsPanel, visualSettingsPanel, controlsSettingsPanel, firstSelectedVisualsSettingsButton, firstSelectedControlsSettingsButton);
        }
        else
        {
            SetPanelsToRightInSettings(soundPauseSettingsPanel, visualPauseSettingsPanel, controlsPauseSettingsPanel, firstSelectedPauseVisualsSettingsButton, firstSelectedPauseControlsSettingsButton);
        }
    }

    private bool AnimationRunning(GameObject _soundSettingsPanel, GameObject _visualSettingsPanel, GameObject _controlsSettingsPanel)
    {
        return IsPlaying(_controlsSettingsPanel.GetComponent<Animator>(), "Panel_in") ||
               IsPlaying(_soundSettingsPanel.GetComponent<Animator>(), "Panel_in") ||
               IsPlaying(_visualSettingsPanel.GetComponent<Animator>(), "Panel_in") || coolDown > 0;
    }

    public void SetPanelsToLeftInSettings(GameObject _soundSettingsPanel, GameObject _visualSettingsPanel, GameObject _controlsSettingsPanel, GameObject firstSelectedButtonSoundSettingsPanel, GameObject firstSelectedButtonVisualSettingsPanel)
    {
        if(AnimationRunning(_soundSettingsPanel, _visualSettingsPanel, _controlsSettingsPanel))
        {
            return;
        }

        if(IsInsideAnimation(_visualSettingsPanel.GetComponent<Animator>(), "Panel_out"))
        {
            _visualSettingsPanel.GetComponent<Animator>().SetTrigger("Pressed");
            _soundSettingsPanel.GetComponent<Animator>().SetTrigger("Pressed");
            SetSelectedButton(firstSelectedButtonSoundSettingsPanel);
            coolDown = 1;
        }
        else if (IsInsideAnimation(_controlsSettingsPanel.GetComponent<Animator>(), "Panel_out"))
        {
            _visualSettingsPanel.GetComponent<Animator>().SetTrigger("Pressed");
            _controlsSettingsPanel.GetComponent<Animator>().SetTrigger("Pressed");
            SetSelectedButton(firstSelectedButtonVisualSettingsPanel);
            coolDown = 1;
        }
    }

    public void SetPanelsToRightInSettings(GameObject _soundSettingsPanel, GameObject _visualSettingsPanel, GameObject _controlsSettingsPanel, GameObject firstSelectedButtonSoundSettingsPanel, GameObject firstSelectedButtonVisualSettingsPanel)
    {
        if (AnimationRunning(_soundSettingsPanel, _visualSettingsPanel, _controlsSettingsPanel))
        {
            return;
        }
        
        if (IsInsideAnimation(_soundSettingsPanel.GetComponent<Animator>(), "Panel_out"))
        {
            _visualSettingsPanel.GetComponent<Animator>().SetTrigger("Pressed");
            _soundSettingsPanel.GetComponent<Animator>().SetTrigger("Pressed");
            SetSelectedButton(firstSelectedButtonSoundSettingsPanel);
            coolDown = 1;
        }
        else if(IsInsideAnimation(_visualSettingsPanel.GetComponent<Animator>(), "Panel_out"))
        {
            _controlsSettingsPanel.GetComponent<Animator>().SetTrigger("Pressed");
            _visualSettingsPanel.GetComponent<Animator>().SetTrigger("Pressed");
            SetSelectedButton(firstSelectedButtonVisualSettingsPanel);
            coolDown = 1;
        }
    }

    #region PAUSE

    public void OnPause()
    {
        if (winPanel.activeSelf || SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainMenu"))
            return;
        
        if (pausePanel.activeSelf)
        {
            AudioManager.Instance.FadeIn("InGameMusic");
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            AudioManager.Instance.FadeOut("InGameMusic");
            SetSelectedButton(firstSelectedPauseButton);
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }   
    }

    public void BackToMain()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }
    
    #endregion

    #region WIN/LOSE

    public void ShowWin(string player)
    {
        SetSelectedButton(firstSelectedWinScreenButton);
        Time.timeScale = 0;
        winPanel.SetActive(true);
        playerText.text = player;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        winPanel.SetActive(false);
        SceneManager.LoadScene("InGame");
    }

    #endregion
}
