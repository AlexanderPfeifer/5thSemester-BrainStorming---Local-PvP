using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddSoundToEveryButton : MonoBehaviour
{
    private Button[] buttons;
    private Slider[] sliders;

    private void Start()
    {
        buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button _button in buttons)
        {
            _button.onClick.AddListener(PlayButtonClickSound);
            AddHoverSound(_button.gameObject);
            AddSelectionSound(_button);
        }
        
        sliders = FindObjectsByType<Slider>(FindObjectsSortMode.None);

        // Attach the value changed sound to each slider
        foreach (Slider _slider in sliders)
        {
            _slider.onValueChanged.AddListener((value) => AudioManager.Instance.Play("SliderOnValueChanged"));
        }
    }

    private void PlayButtonClickSound()
    {
        AudioManager.Instance.Play("ButtonClick");
    }
    
    private void AddHoverSound(GameObject buttonObject)
    {
        // Add an EventTrigger component if it doesn't already exist
        EventTrigger _eventTrigger = buttonObject.GetComponent<EventTrigger>();
        if (_eventTrigger == null)
        {
            _eventTrigger = buttonObject.AddComponent<EventTrigger>();
        }

        // Create a new entry for the PointerEnter event (hover)
        EventTrigger.Entry _entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter // PointerEnter is triggered when the pointer hovers over the button
        };

        // Add a listener to play the hover sound
        _entry.callback.AddListener((eventData) => AudioManager.Instance.Play("ButtonHover"));
        _eventTrigger.triggers.Add(_entry);
    }
    
    private void AddSelectionSound(Button button)
    {
        // Add an EventTrigger component if it doesn't already exist
        EventTrigger _eventTrigger = button.gameObject.GetComponent<EventTrigger>();
        if (_eventTrigger == null)
        {
            _eventTrigger = button.gameObject.AddComponent<EventTrigger>();
        }

        // Create a new entry for the Select event
        EventTrigger.Entry _entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.Select // This event triggers when the button is selected
        };

        // Add a listener to play the selected sound
        _entry.callback.AddListener((eventData) => AudioManager.Instance.Play("ButtonHover"));
        _eventTrigger.triggers.Add(_entry);
    }
}
