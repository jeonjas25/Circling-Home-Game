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
    public Transform firePoint1;
    public Transform firePoint2;
    public float scrollSpeed = 5f;
    public Material laserMaterial;
    public SpriteRenderer laserSprite1;
    public SpriteRenderer laserSprite2;
    public LayerMask damageLayer;

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
        currentFireCooldown += Time.deltaTime;
        if (currentFireCooldown >= fireCooldown)
        {
            currentState = LaserState.Patrolling;
            laserSprite1.gameObject.SetActive(false);
            laserSprite2.gameObject.SetActive(false);
            currentFireCooldown = 0;
        }

        if (currentFireCooldown < 0.1f)
        {
            FireLaser();
        }
    }

    void FireLaser()
    {
        // First Eye
        Vector2 fireDirection1 = transform.localScale.x > 0 ? firePoint1.right : -firePoint1.right;
        RaycastHit2D hit1 = Physics2D.Raycast(firePoint1.position, fireDirection1, Mathf.Infinity, damageLayer);
        Debug.DrawRay(firePoint1.position, fireDirection1 * 10, Color.yellow, 0.2f); 

        if (hit1.collider != null)
        {
            laserSprite1.transform.localScale = new Vector3(hit1.distance, laserSprite1.transform.localScale.y, laserSprite1.transform.localScale.z);
            laserMaterial.mainTextureOffset += new Vector2(scrollSpeed * Time.deltaTime, 0);
            laserSprite1.gameObject.SetActive(true);
        }
        else
        {
            laserSprite1.transform.localScale = new Vector3(0, laserSprite1.transform.localScale.y, laserSprite1.transform.localScale.z);
        }

        // Second Eye
        Vector2 fireDirection2 = transform.localScale.x > 0 ? firePoint2.right : -firePoint2.right;
        RaycastHit2D hit2 = Physics2D.Raycast(firePoint2.position, fireDirection2, Mathf.Infinity, damageLayer);
        Debug.DrawRay(firePoint2.position, fireDirection2 * 10, Color.yellow, 0.2f);
        if (hit2.collider != null)
        {
            laserSprite2.transform.localScale = new Vector3(hit2.distance, laserSprite2.transform.localScale.y, laserSprite2.transform.localScale.z);
            laserMaterial.mainTextureOffset += new Vector2(scrollSpeed * Time.deltaTime, 0);
            laserSprite2.gameObject.SetActive(true);
        }
        else
        {
            laserSprite2.transform.localScale = new Vector3(0, laserSprite2.transform.localScale.y, laserSprite2.transform.localScale.z);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
