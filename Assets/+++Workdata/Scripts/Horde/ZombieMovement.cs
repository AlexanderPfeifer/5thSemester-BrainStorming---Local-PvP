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

    [SerializeField] public LayerMask ownZombieLayer;
    [SerializeField] private float ownZombiesRadius;
    [SerializeField] private float closeEnoughToMousePositionRadius;
    private Transform closestZombieOwnTeam;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if(GetComponent<Health>().isDead)
            return;
        
        if(!GetComponent<AutoAttack>().isAttacking)
            MoveHordeToMouse();
        
        KeepDistanceToOwnZombies();

        MoveAnimationUpdate();
    }

    private void MoveHordeToMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 10;
        }
        
        transform.position = Vector3.MoveTowards(transform.position, mousePos, Time.deltaTime * speed);
    }
    
    private void KeepDistanceToOwnZombies()
    {
        Collider2D[] zombieTooCloseHit = Physics2D.OverlapCircleAll(transform.position, ownZombiesRadius, ownZombieLayer);

        //Compares length to 1, because the OverlapCircleAll always hits itself first, then other zombies
        if (zombieTooCloseHit.Length > 1)
        {
            closestZombieOwnTeam = zombieTooCloseHit[0].transform;

            foreach (Collider2D zombie in zombieTooCloseHit)
            {
                if (zombie == GetComponent<Collider2D>()) 
                    continue; // Skip the current object's collider
                
                if ((zombie.transform.position - transform.position).sqrMagnitude < (closestZombieOwnTeam.transform.position - transform.position).sqrMagnitude)
                {
                    closestZombieOwnTeam = zombie.transform;
                }
            }
            
            //Moves away from the closest zombie
            transform.position = Vector3.MoveTowards(transform.position,  closestZombieOwnTeam.position, -1 * speed * Time.deltaTime);
        }
    }

    private void MoveAnimationUpdate()
    {
        // Calculate the speed based on the distance moved
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        currentSpeed = distanceMoved / Time.deltaTime;  // Speed = distance / time

        // Store the current position for the next frame
        lastPosition = transform.position;

        anim.SetFloat("moveSpeed", currentSpeed);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, ownZombiesRadius);        
        
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, closeEnoughToMousePositionRadius);
    }
}
