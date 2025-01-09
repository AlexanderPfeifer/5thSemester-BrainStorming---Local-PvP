using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Grouping")]
    private Vector2 moveInput;
    [DisplayColor(0, 0, 1), SerializeField] public float groupingRadius;

    [Header("Movement")]
    [SerializeField] private float baseMoveSpeed;
    [SerializeField] private float speedSmoothTime = 1.0f;
    private Vector3 lastPosition;
    private Vector3 currentVelocity = new Vector3(0, 0, 0);
    private bool movementAllowed;


    private CachedZombieData cachedZombieData;

    private void Start()
    {
        cachedZombieData = GetComponent<CachedZombieData>();
    }

    private void OnEnable()
    {
        PlayerRegistryManager.Instance.AllPlayersReady += OnAllowMovement;
    }
    
    private void OnDisable()
    {
        PlayerRegistryManager.Instance.AllPlayersReady -= OnAllowMovement;
    }

    private void Update()
    {
        if(movementAllowed)
            MoveZombie();
    }

    private void LateUpdate()
    {
        if (!movementAllowed || cachedZombieData.Health.isDead)
            return;

        MoveAnimationLateUpdate();
    }

    private void OnAllowMovement()
    {
        StartCoroutine(DelayedMovementAllowance());
    }

    private IEnumerator DelayedMovementAllowance()
    {
        yield return new WaitForSeconds(3);

        movementAllowed = true;
    }

    public void OnMove(InputValue inputValue)
    {
        if (cachedZombieData.Health.isDead)
        {
            moveInput = Vector2.zero;
        }
        else
        {
            moveInput = inputValue.Get<Vector2>().normalized;
        }
    }

    void MoveZombie()
    {
        transform.position = Vector3.SmoothDamp(transform.position, (Vector2)transform.position + (moveInput * baseMoveSpeed).normalized, ref currentVelocity, speedSmoothTime);
    }

    void MoveAnimationLateUpdate()
    {
        var currentSpeed = Vector3.Distance(transform.position, lastPosition) / Time.deltaTime;

        lastPosition = transform.position;

        cachedZombieData.Animator.SetFloat("moveSpeed", currentSpeed);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, groupingRadius);
    }
}
