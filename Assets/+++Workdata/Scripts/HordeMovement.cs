using System;
using UnityEngine;
using UnityEngine.Serialization;

public class HordeMovement : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    private Vector3 mousePos;
    
    //Work with how loud the horde is? The faster it is, the more they get noticed

    [SerializeField] private float currentHordeSpeed;

    [SerializeField] private float minHordeSpeed;
    [SerializeField] private float hordeSpeedIncreaseOnClick;
    [SerializeField] private float maxHordeSpeed;
    [SerializeField] private AnimationCurve movementSpeedDecrease;

    private void Update()
    {
        MoveHordeToMouse();
    }

    private void MoveHordeToMouse()
    {
        if (currentHordeSpeed > minHordeSpeed)
        {
            currentHordeSpeed -= Time.deltaTime * movementSpeedDecrease.Evaluate(currentHordeSpeed);
        }
        else
        {
            currentHordeSpeed = minHordeSpeed;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if(currentHordeSpeed < maxHordeSpeed)
            {
                currentHordeSpeed += hordeSpeedIncreaseOnClick;
            }
            else
            {
                currentHordeSpeed = maxHordeSpeed;
            }
        }
        
        if (Input.GetMouseButton(0))
        {
            mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 10;
            
            transform.position = Vector3.MoveTowards(transform.position, mousePos, Time.deltaTime * currentHordeSpeed);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, mousePos, Time.deltaTime * currentHordeSpeed);
        }

        if (Vector3.Distance(transform.position, mousePos) < Mathf.Epsilon)
        {
            currentHordeSpeed = minHordeSpeed;
        }
    }
}
