using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    [Header("Animations")]
    private Animator anim;
    
    [Header("Input")]
    [SerializeField] private Camera mainCam;
    private Vector3 mousePos = new(0,0,10);

    [Header("Speed")]
    [SerializeField] public float speed;
    private Vector3 lastPosition;
    private float currentSpeed;

    [Header("Seperation")]
    [SerializeField] public LayerMask ownZombieLayer;
    [SerializeField] private float seperationRadius;
    [SerializeField] private float seperationSpeed = 1;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if(GetComponent<Health>().isDead)
            return;

        MoveZombie();
    }

    private void LateUpdate()
    {
        if (GetComponent<Health>().isDead)
            return;

        MoveAnimationLateUpdate();
    }

    private void MoveZombie()
    {
        transform.position += (CalculateMoveToMouseForce() + (CalculateSeparationForce() * seperationSpeed)) * Time.deltaTime * speed;
    }

    private Vector3 CalculateMoveToMouseForce()
    {
        if (GetComponent<AutoAttack>().isAttacking)
        {
            // Set mouseposition to transformPosition because it shouldn't move after having defeated the enemy
            mousePos = transform.position; 
            return Vector3.zero;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 10;
        }

        // Stop moving if close enough to the mouse position
        float stopDistance = 0.1f; // Adjust this threshold as needed
        if (Vector3.Distance(transform.position, mousePos) <= stopDistance)
        {
            return Vector3.zero;
        }

        return (mousePos - transform.position).normalized;
    }

    private Vector3 CalculateSeparationForce()
    {
        Collider2D[] zombieTooCloseHit = Physics2D.OverlapCircleAll(transform.position, seperationRadius, ownZombieLayer);

        Vector3 _separationForce = Vector3.zero;

        // Ignore separation if there's no other zombie nearby and compare to one because overlapCircle always hits itself
        if (zombieTooCloseHit.Length <= 1)
        {
            return _separationForce;
        }

        foreach (Collider2D zombie in zombieTooCloseHit)
        {
            if (zombie == GetComponent<Collider2D>())
                continue; 

            Vector3 oppositeDirectionToNearZombie = transform.position - zombie.transform.position;

            // Compare to more than 0 to avoid division by 0
            if (oppositeDirectionToNearZombie.magnitude > 0) 
            {
                //Adds all the directions opposite of the near zombies to get the best path of where to go
                _separationForce += oppositeDirectionToNearZombie.normalized / oppositeDirectionToNearZombie.magnitude; // Stronger repulsion when closer
            }
        }

        return _separationForce.normalized;
    }

    private void MoveAnimationLateUpdate()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        currentSpeed = distanceMoved / Time.deltaTime;  

        lastPosition = transform.position;

        anim.SetFloat("moveSpeed", currentSpeed);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, seperationRadius);

        // Draw the stop distance
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(mousePos, 0.1f); // Same as stopDistance
    }
}
