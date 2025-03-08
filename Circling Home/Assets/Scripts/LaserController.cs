using UnityEngine;

public class LaserController : MonoBehaviour
{
    public enum LaserState {
        Patrolling,
        Charging,
        Firing
    }
    public LaserState currentState = LaserState.Patrolling;

    // patrol variables
    public float patrolSpeed;
    private int patrolDirection = 1;
    public float raycastDistance = 1.5f;
    public LayerMask platformLayer;
    public Vector2 raycastOffset = new Vector2(0.5f, -1.9f);

    // charge variables
    public float chargeTime = 1f;
    private float currentChargeTime = 0f;
    public float detectionRange = 5f;
    public LayerMask playerLayer;

    // fire variables
    public float fireCooldown = 2f;
    private float currentFireCooldown = 0f;

    // other variables
    private Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case LaserState.Patrolling:
                PatrolDetection();
                break;
            case LaserState.Charging:
                Charge();
                break;
            case LaserState.Firing:
                Fire();
                break;
        }
    }

    void FixedUpdate()
    {
        if (currentState == LaserState.Patrolling)
        {
            PatrolMovement();
        }
    }

    void PatrolMovement()
    {
        rb.linearVelocity = new Vector2(patrolSpeed * patrolDirection, rb.linearVelocity.y);
    }

    void PatrolDetection()
    {
        // detecting edge of platform
        Vector2 raycastPosition = (Vector2) transform.position + new Vector2(raycastOffset.x * patrolDirection, raycastOffset.y);
        RaycastHit2D hit = Physics2D.Raycast(raycastPosition, Vector2.down, raycastDistance, platformLayer);
        Debug.DrawRay(raycastPosition, Vector2.down * raycastDistance, Color.red);

        if (hit.collider == null) 
        {
            patrolDirection *= -1;
        }
        
        // detecting player
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        if ((player != null) && (currentState == LaserState.Patrolling))
        {
            currentState = LaserState.Charging;
            currentChargeTime = 0f;
        }
    }

    void Charge()
    {
        currentChargeTime += Time.deltaTime;
        if (currentChargeTime >= chargeTime)
        {
            currentState = LaserState.Firing;
            currentFireCooldown = 0f;
        }
    }

    void Fire()
    {

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
