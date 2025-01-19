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

        eventTrigger.OnPointerEnter(_pointerEventData);
    }
    
    public void OnDeselect(BaseEventData eventData)
    {
        PointerEventData _pointerEventData = new PointerEventData(EventSystem.current);

        eventTrigger.OnPointerExit(_pointerEventData);
    }
}
