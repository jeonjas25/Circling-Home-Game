using UnityEngine;
using System.Collections.Generic;

public class ChainLightningBoltController : MonoBehaviour
{
    public float speed = 20f;
    public int maxChains = 3;
    public float chainRange = 10f; 
    public float damageAmount = 10f;
    public float slowAmount = 0.5f; // Percentage to slow down (e.g., 0.5 means 50% speed)
    public float slowDuration = 2f; // How long the slow lasts
    public Rigidbody2D rb;
    public Vector2 direction;
    public float maxXPos = 10f;
    private float playerInitialX;

    private Transform targetEnemy; // The first enemy we're aiming at
    private int currentChains = 0;
    private List<GameObject> hitEnemies; // To prevent hitting the same enemy multiple times
    private SpriteRenderer spriteRenderer;
    private GameObject firstEnemyHit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hitEnemies = new List<GameObject>();
        playerInitialX = transform.position.x;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            if (direction.x > 0)
            {
                spriteRenderer.flipX = true; 
            }
        }

        rb.linearVelocity = direction * speed;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Mathf.Abs(transform.position.x - playerInitialX);
        if (distance > maxXPos)
        {
            Destroy(gameObject);
        }

        if (targetEnemy != null)
        {
            // Move towards the current target enemy
            transform.position = Vector2.MoveTowards(transform.position, targetEnemy.position, speed * Time.deltaTime);

            // Check if we've reached the target
            if (Vector2.Distance(transform.position, targetEnemy.position) < 0.1f)
            {
                HitEnemy(targetEnemy.gameObject);
            }
        }

        else if (currentChains < maxChains && hitEnemies.Count > 0)
        {
            FindNewTarget();
        }

        else if (currentChains >= maxChains)
        {
            Destroy(gameObject); // Destroy after max chains
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser") && !hitEnemies.Contains(other.gameObject))
        {
            Debug.Log("Lightning Hit first Laser");
            firstEnemyHit = other.gameObject;
            HitEnemy(firstEnemyHit);
        }
    }

    void HitEnemy(GameObject enemyObject)
    {
        if (enemyObject.CompareTag("Laser"))
        {
            LaserController laserController = enemyObject.GetComponent<LaserController>();
            if (laserController != null)
            {
                laserController.TakeDamage(damageAmount);
                Debug.Log("Dealt " + damageAmount + " damage to " + enemyObject.name);
            }

            // add slow effect and slow projectile stuff here
        }

        hitEnemies.Add(enemyObject);
        currentChains++;
        targetEnemy = null; // Reset target to find the next one

        if (currentChains < maxChains)
        {
            FindNewTarget();
        }
        else
        {
            Destroy(gameObject); // We've chained enough times
        }
    }

    void FindNewTarget()
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, chainRange);
        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (Collider2D enemyCollider in nearbyEnemies)
        {
            if (enemyCollider.CompareTag("Laser") && !hitEnemies.Contains(enemyCollider.gameObject))
            {
                float distance = Vector2.Distance(transform.position, enemyCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemyCollider.gameObject;
                }
            }
        }

        if (closestEnemy != null)
        {
            targetEnemy = closestEnemy.transform;
            Debug.Log("Targeting new enemy: " + closestEnemy.name);
        }
        else
        {
            Debug.Log("No new targets found.");
            Destroy(gameObject, 1f); // Destroy if no new targets after a short delay
        }
    }
}
