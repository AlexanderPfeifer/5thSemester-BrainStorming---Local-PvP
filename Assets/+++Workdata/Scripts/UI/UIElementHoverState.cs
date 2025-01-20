using UnityEngine;
using UnityEngine.EventSystems;

public class UIElementHoverState : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private EventTrigger eventTrigger;
    
    private void Awake() {
        eventTrigger = GetComponent<EventTrigger>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        PointerEventData _pointerEventData = new PointerEventData(EventSystem.current);

        foreach (EventTrigger.Entry entry in eventTrigger.triggers)
        {
            // Check for PointerEnter trigger
            if (entry.eventID == EventTriggerType.PointerEnter)
            {
                eventTrigger.OnPointerEnter(_pointerEventData);
            }

            // Check for Select trigger
            if (entry.eventID == EventTriggerType.Select)
            {
                eventTrigger.OnSelect(_pointerEventData);
            }
        }
    }
    
    public void OnDeselect(BaseEventData eventData)
    {
        PointerEventData _pointerEventData = new PointerEventData(EventSystem.current);
        
        foreach (EventTrigger.Entry entry in eventTrigger.triggers)
        {
            // Check for PointerEnter trigger
            if (entry.eventID == EventTriggerType.PointerEnter)
            {
                eventTrigger.OnPointerExit(_pointerEventData);
            }

            // Check for Select trigger
            if (entry.eventID == EventTriggerType.Select)
            {
                eventTrigger.OnDeselect(_pointerEventData);
            }
        }
    }
}
