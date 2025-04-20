using UnityEngine;

public class ChargerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float chargeSpeedMultiplier = 1.5f;
    public float chargeDelay = 1f;
    private float currentChargeDelay = 0f;
    private bool isCharging = false;
    private Transform playerTarget;
    public float damageAmount = 10f;
    public float collisionDisableTime = 0.5f;
    public float detectionRange = 15f;

    private Rigidbody2D rb;
    private Collider2D enemyCollider;
    private float lastCollisionTime = -Mathf.Infinity;

    public Sprite sleepingSprite; 
    public Sprite angrySprite;
    private SpriteRenderer spriteRenderer;

    public float maxHealth = 30f;
    public float currentHealth;
    public EnemyHealthBar healthBar;

    public float slowPercentage = 0f;
    public float slowTimer = 0f;
    public bool isSlowed = false;
    public float originalSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        FindPlayer();
        spriteRenderer.sprite = sleepingSprite;
        healthBar.SetMaxHealth(maxHealth);
        currentHealth = maxHealth;
        originalSpeed = moveSpeed;
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTarget = player.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTarget == null)
        {
            FindPlayer();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Player detected, switch to angry sprite
            spriteRenderer.sprite = angrySprite;

            float targetX = playerTarget.position.x;
            float currentX = transform.position.x;

            float moveDirection = Mathf.Sign(targetX - currentX);

            if (!isCharging)
            {
                if (isSlowed)
                {
                    moveSpeed = originalSpeed * (1f - slowPercentage);
                    slowTimer -= Time.deltaTime;
                    if (slowTimer <= 0)
                    {
                        isSlowed = false;
                        slowPercentage = 0f;
                        moveSpeed = originalSpeed;
                        Debug.Log("Slow Stopped");
                    }
                }
                else 
                {
                    moveSpeed = originalSpeed;
                }
                
                // Move towards the player
                rb.linearVelocity = new Vector2(moveDirection * moveSpeed, 0f);

                // Start charging after a delay
                currentChargeDelay += Time.deltaTime;
                if (currentChargeDelay >= chargeDelay)
                {
                    isCharging = true;
                    currentChargeDelay = 0f;
                }
            }
            else
            {
                if (isSlowed)
                {
                    moveSpeed = originalSpeed * (1f - slowPercentage);
                    slowTimer -= Time.deltaTime;
                    if (slowTimer <= 0)
                    {
                        isSlowed = false;
                        slowPercentage = 0f;
                        moveSpeed = originalSpeed;
                        Debug.Log("Slow Stopped");
                    }
                }
                else 
                {
                    moveSpeed = originalSpeed;
                }

                // Charge towards the player
                rb.linearVelocity = new Vector2(moveDirection * moveSpeed * chargeSpeedMultiplier, 0f);

                // Stop charging if close enough (optional)
                if (Vector2.Distance(transform.position, playerTarget.position) < 1f)
                {
                    isCharging = false;
                }
            }
        }
        else
        {
            // Player not detected, switch to sleeping sprite and stop moving
            spriteRenderer.sprite = sleepingSprite;
            rb.linearVelocity = Vector2.zero;
            isCharging = false;
            currentChargeDelay = 0f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time - lastCollisionTime > collisionDisableTime)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(damageAmount);
                    lastCollisionTime = Time.time;
                }
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        healthBar.SetHealth(currentHealth);
        Debug.Log("Damage: " + damageAmount);
        if (currentHealth <= 0)
        {
            Die();
            Debug.Log("Charger died!");
        }
    }

    public void SlowDown(float percentage, float duration)
    {
        Debug.Log("Charger Slowed");
        slowPercentage = percentage;
        slowTimer = duration;
        isSlowed = true;
    }   

    void Die()
    {
        Destroy(gameObject);
    }
}
