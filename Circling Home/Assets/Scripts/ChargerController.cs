using UnityEngine;
using System.Collections.Generic; 

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
    public float verticalTrackThreshold = 2f;

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

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Sprite initialSprite;
    private static List<ChargerController> existingChargers = new List<ChargerController>();

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

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialSprite = sleepingSprite;
        existingChargers.Add(this);
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
        float verticalDistance = Mathf.Abs(playerTarget.position.y - transform.position.y);

        if (distanceToPlayer <= detectionRange && verticalDistance <= verticalTrackThreshold)
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
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        Debug.Log(gameObject.name + " - Charger removed from existingChargers");
        existingChargers.Remove(this);
    }

    void OnEnable()
    {
        if (!existingChargers.Contains(this))
        {
            Debug.Log(gameObject.name + " - Charger added to existingChargers");
            existingChargers.Add(this);
        }
    }

    public void ResetEnemy()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        spriteRenderer.sprite = initialSprite;
        currentHealth = maxHealth;
        isCharging = false;
        currentChargeDelay = 0f;
        isSlowed = false;
        slowPercentage = 0f;
        slowTimer = 0f;
        moveSpeed = originalSpeed;
        rb.linearVelocity = Vector2.zero;
        enemyCollider.enabled = true;
        gameObject.SetActive(true);

        FindPlayer(); // Ensure it tries to find the player again
        healthBar.SetHealth(maxHealth);
    }

    public static void ResetAllChargers()
    {
        Debug.Log("ResetAllChargers() called. List count: " + existingChargers.Count);
        foreach (ChargerController charger in existingChargers)
        {
            if (charger == null)
            {
                Debug.LogError("Found a null entry in existingChargers!");
                continue;
            }
            Debug.Log("Attempting to reset: " + charger.gameObject.name);
            charger.ResetEnemy();
            Debug.Log(charger.gameObject.name + " - ResetEnemy() called.");
        }
    }
}
