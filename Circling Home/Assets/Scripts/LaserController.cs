using UnityEngine;
using System.Collections;

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
    public float fireRate = 0.5f;
    private float lastFireTime = 0f;
    public float fireCooldown = 2f;
    private float currentFireCooldown = 0f;
    public Transform firePoint1;
    public Transform firePoint2;
    public float scrollSpeed = 5f;
    public LayerMask damageLayer;
    public GameObject bulletPrefab;
    public GameObject homingBulletPrefab;
    private int bulletCount = 1;

    // other variables
    private Rigidbody2D rb;
    private RigidbodyType2D initialBodyType;
    public float maxHealth = 50f;
    public float currentHealth;
    public EnemyHealthBar healthBar;
    public GameObject healPotionPrefab;
    public float potionDropChance = 0.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        initialBodyType = rb.bodyType;
        healthBar.SetMaxHealth(maxHealth);

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
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
        // changing sprite direction based on patrol direction
        if (patrolDirection > 0)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z); // Face right
        }
        else
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z); // Face left
        }

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
        rb.bodyType = RigidbodyType2D.Static;
        currentChargeTime += Time.deltaTime;
        if (currentChargeTime >= chargeTime)
        {
            currentState = LaserState.Firing;
            currentFireCooldown = 0f;
        }
    }

    void Fire()
    {
        rb.bodyType = RigidbodyType2D.Static;
        currentFireCooldown += Time.deltaTime;
        if (currentFireCooldown >= fireCooldown)
        {
            currentState = LaserState.Patrolling;
            rb.bodyType = initialBodyType;
            currentFireCooldown = 0;
        }
            FireLaser();
    }

    void FireLaser()
    {
        if (Time.time - lastFireTime >= fireRate)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                Vector2 playerPosition = player.transform.position;
                float horizontalOffset = playerPosition.x - transform.position.x;
                Vector2 directionToPlayer = new Vector2(horizontalOffset, 0f).normalized;

                if (directionToPlayer.x > 0)
                {
                    transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                }
                else
                {
                    transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                }

                if ((bulletCount % 3 == 0) && (bulletCount != 0))
                {

                    GameObject homingLaserBullet = Instantiate(homingBulletPrefab, firePoint2.position, Quaternion.identity);

                    HomingLaserBulletController homingBulletController = homingLaserBullet.GetComponent<HomingLaserBulletController>();

                    if (homingBulletController != null)
                    {
                        homingBulletController.direction = directionToPlayer;
                    }

                    Collider2D enemyCollider = GetComponent<Collider2D>();
                    Transform homingLaserChildTransform = homingLaserBullet.transform.Find("Collider");
                    Collider2D homingLaserCollider = homingLaserChildTransform.GetComponent<Collider2D>();

                    Physics2D.IgnoreCollision(enemyCollider, homingLaserCollider, true);

                    StartCoroutine(EnableCollision(enemyCollider, homingLaserCollider));
                }
                else 
                {
                    GameObject laserBullet = Instantiate(bulletPrefab, firePoint1.position, Quaternion.identity);

                    LaserBulletController bulletController = laserBullet.GetComponent<LaserBulletController>();

                    if (bulletController != null)
                    {
                        bulletController.direction = directionToPlayer;
                    }

                    Collider2D enemyCollider = GetComponent<Collider2D>();
                    Collider2D laserCollider = laserBullet.GetComponent<Collider2D>();

                    Physics2D.IgnoreCollision(enemyCollider, laserCollider, true);

                    StartCoroutine(EnableCollision(enemyCollider, laserCollider));
                    
                }

                lastFireTime = Time.time; // Reset the timer
                bulletCount++;
            }

    }

    IEnumerator EnableCollision(Collider2D enemyCollider, Collider2D laserCollider)
    {
        yield return new WaitForSeconds(0.2f); // Adjust the delay as needed
        if (laserCollider != null)
        {
            Physics2D.IgnoreCollision(enemyCollider, laserCollider, false); // Re-enable collision
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
            Debug.Log("Laser died!");
        }
    }

    void Die()
    {
        if (Random.value <= potionDropChance && healPotionPrefab != null)
        {
            Instantiate(healPotionPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
