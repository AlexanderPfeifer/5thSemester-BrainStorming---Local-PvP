using UnityEngine;
using UnityEngine.EventSystems;

public class HumanHordeMovementDirection : MonoBehaviour
{
    private void Start()
    {
        ChangeMoveDirection();
    }

    private void ChangeMoveDirection()
    {
        var moveDir = Random.insideUnitCircle;

        foreach (Transform child in transform)
        {
            child.GetComponent<NPCMovement>().moveDirection = moveDir;
        }
    }
}
