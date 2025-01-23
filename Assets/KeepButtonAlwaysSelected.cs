using UnityEngine;
using UnityEngine.EventSystems;

public class KeepButtonAlwaysSelected : MonoBehaviour
{
    private GameObject lastSelectedButton; 
    private EventSystem eventSystem;

    void Start()
    {
        eventSystem = EventSystem.current;
    }

    void Update()
    {
        if (eventSystem.currentSelectedGameObject == null)
        {
            if (lastSelectedButton != null)
            {
                eventSystem.SetSelectedGameObject(lastSelectedButton);
            }
        }
        else
        {
            lastSelectedButton = eventSystem.currentSelectedGameObject;
        }
    }
}
