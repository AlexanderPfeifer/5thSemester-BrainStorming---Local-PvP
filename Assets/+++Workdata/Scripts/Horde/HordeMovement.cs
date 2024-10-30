using System;
using UnityEngine;
using UnityEngine.Serialization;

public class HordeMovement : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    private Vector3 mousePos;
    
    //Work with how loud the horde is? The faster it is, the more they get noticed

    public float CurrentHordeSpeed { get; private set; }
    public bool StopMovement { get; set; }

    [SerializeField] private float minHordeSpeed;
    [SerializeField] private float hordeSpeedIncreaseOnClick;
    [SerializeField] private float maxHordeSpeed;
    [SerializeField] private AnimationCurve movementSpeedDecrease;
    
    

    private void Update()
    {
        if(StopMovement)
            return;
        
        MoveHordeToMouse();
    }

    private void MoveHordeToMouse()
    {
        if (CurrentHordeSpeed > minHordeSpeed)
        {
            CurrentHordeSpeed -= Time.deltaTime * movementSpeedDecrease.Evaluate(CurrentHordeSpeed);
        }
        else
        {
            CurrentHordeSpeed = minHordeSpeed;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if(CurrentHordeSpeed < maxHordeSpeed)
            {
                CurrentHordeSpeed += hordeSpeedIncreaseOnClick;
            }
            else
            {
                CurrentHordeSpeed = maxHordeSpeed;
            }
        }
        
        if (Input.GetMouseButton(0))
        {
            mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 10;
            
            transform.position = Vector3.MoveTowards(transform.position, mousePos, Time.deltaTime * CurrentHordeSpeed);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, mousePos, Time.deltaTime * CurrentHordeSpeed);
        }

        if (Vector3.Distance(transform.position, mousePos) < Mathf.Epsilon)
        {
            CurrentHordeSpeed = minHordeSpeed;
        }
    }
}
