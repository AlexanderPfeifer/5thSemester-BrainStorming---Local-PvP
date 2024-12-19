using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ZombieMovement : MonoBehaviour
{
    [SerializeField] private bool isPlayer;

    [Header("Movement")]
    [SerializeField] private float baseMoveSpeed;
    [SerializeField] private float baseSpeedOffset;
    [SerializeField] private float speedSmoothTime = 1.0f;
    private Vector3 lastPosition;
    private Vector3 currentVelocity = new Vector3(0, 0, 0);
    private Vector2 moveInput;

    [Header("Seperation")]
    [SerializeField] private float seperationSpeed = 1;
    [DisplayColor(0, 0, 1), SerializeField] private float seperationRadius;
    private Collider2D[] seperationZombies;

    [Header("Grouping")]
    [SerializeField] private float groupCenterSpeed = 1;

    private CachedZombieData cachedZombieData;

    private void Start()
    {
        cachedZombieData = GetComponent<CachedZombieData>();
        baseMoveSpeed -= Random.Range(-baseSpeedOffset, baseSpeedOffset);
    }

    private void Update()
    {
        MoveZombie();
    }

    private void LateUpdate()
    {
        if (cachedZombieData.Health.isDead)
            return;

        MoveAnimationLateUpdate();
    }

    void MoveZombie()
    {
        Vector2 moveDirection = (moveInput * baseMoveSpeed) + (SeparationForce() * seperationSpeed);

        if (IsOutOfSeperationDeadZone())
        {
            moveDirection += (GetGroupCenter() - (Vector2)transform.position).normalized * groupCenterSpeed;
        }

        transform.position = Vector3.SmoothDamp(transform.position, (Vector2)transform.position + moveDirection.normalized, ref currentVelocity, speedSmoothTime);
    }

    public void OnMove(InputValue inputValue)
    {
        if (cachedZombieData.AutoAttack.isAttacking || cachedZombieData.Health.isDead)
        {
            moveInput = Vector2.zero;
        }
        else
        {
            moveInput = inputValue.Get<Vector2>().normalized;
        }
    }

    Vector2 SeparationForce()
    {
        seperationZombies = Physics2D.OverlapCircleAll(transform.position, seperationRadius, 1 << gameObject.layer);

        Vector2 _separationForce = Vector2.zero;

        // Ignore separation if there's no other zombie nearby and compare to one because overlapCircle always hits itself
        if (seperationZombies.Length <= 1)
        {
            return _separationForce;
        }

        foreach (Collider2D zombie in seperationZombies)
        {
            if (zombie == GetComponent<Collider2D>())
                continue; 

            Vector2 oppositeDirectionToNearZombie = transform.position - zombie.transform.position;

            // Compare to more than 0 to avoid division by 0
            if (oppositeDirectionToNearZombie.magnitude > 0) 
            {
                _separationForce += oppositeDirectionToNearZombie / oppositeDirectionToNearZombie.magnitude; // Stronger repulsion when closer
            }
        }

        return _separationForce;
    }

    bool IsOutOfSeperationDeadZone()
    {
        foreach (Collider2D zombie in seperationZombies)
        {
            if (zombie == GetComponent<Collider2D>())
                continue;

            float distance = Vector2.Distance(transform.position, zombie.transform.position);

            // Return true if zombies are within 80% of the separation radius (dead zone)
            if (distance <= seperationRadius * 0.8f)
            {
                return true;
            }
        }

        return false;
    }

    Vector2 GetGroupCenter()
    {
        var _zombies = cachedZombieData.ZombiePlayerHordeRegistry.Zombies;

        if (!isPlayer)
        {
            //_zombies = 
        }

        List<float> xPositions = new List<float>();
        List<float> yPositions = new List<float>();

        foreach (GameObject zombie in cachedZombieData.ZombiePlayerHordeRegistry.Zombies)
        {
            xPositions.Add(zombie.transform.position.x);
            yPositions.Add(zombie.transform.position.y);
        }

        xPositions.Sort();
        yPositions.Sort();

        float medianX = (xPositions.Count % 2 == 1) ? xPositions[xPositions.Count / 2] : (xPositions[xPositions.Count / 2 - 1] + xPositions[xPositions.Count / 2]) / 2;

        float medianY = (yPositions.Count % 2 == 1) ? yPositions[yPositions.Count / 2] : (yPositions[yPositions.Count / 2 - 1] + yPositions[yPositions.Count / 2]) / 2;


        return new Vector2(medianX, medianY);
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
        Gizmos.DrawWireSphere(transform.position, seperationRadius);
    }
}
