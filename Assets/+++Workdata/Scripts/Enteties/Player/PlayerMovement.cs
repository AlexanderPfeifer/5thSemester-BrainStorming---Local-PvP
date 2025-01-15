using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Grouping")]
    private Vector2 moveInput;
    [DisplayColor(0, 0, 1), SerializeField] public float groupingRadius;

    [Header("Movement")] 
    private float currentMoveSpeed;
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
        if(movementAllowed)
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
        moveInput = inputValue.Get<Vector2>().normalized;
    }

    void MoveZombie()
    {
        float _speedSubtraction = cachedZombieData.ZombiePlayerHordeRegistry.Zombies.Count * .5f;

        currentMoveSpeed = baseMoveSpeed - Mathf.Clamp(_speedSubtraction, .5f, baseMoveSpeed - 3);
        
        transform.position = Vector3.SmoothDamp(transform.position, transform.position + (new Vector3(moveInput.x, 0, moveInput.y) * currentMoveSpeed).normalized, ref currentVelocity, speedSmoothTime);
    }

    void MoveAnimationLateUpdate()
    {
        var _currentSpeed = Vector3.Distance(transform.position, lastPosition) / Time.deltaTime;

        lastPosition = transform.position;

        cachedZombieData.Animator.SetFloat("moveSpeed", _currentSpeed);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, groupingRadius);
    }
}
