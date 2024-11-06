using System;
using System.Collections.Generic;
using UnityEngine;

public class HordeMovement : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    private Vector3 mousePos;
    [SerializeField] private List<Animator> zombieAnimatorList;
    
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
            
            foreach (var zombieAnimator in zombieAnimatorList)
            {
                zombieAnimator.SetBool("isMoving", true);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, mousePos, Time.deltaTime * CurrentHordeSpeed);
            
            foreach (var zombieAnimator in zombieAnimatorList)
            {
                zombieAnimator.SetBool("isMoving", true);
            }
        }

        if (Vector3.Distance(transform.position, mousePos) < Mathf.Epsilon)
        {
            foreach (var zombieAnimator in zombieAnimatorList)
            {
                zombieAnimator.SetBool("isMoving", false);
            }
            CurrentHordeSpeed = minHordeSpeed;
        }
    }
}
