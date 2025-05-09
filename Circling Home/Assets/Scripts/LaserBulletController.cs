using UnityEngine;

public class LaserBulletController : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;
    public Vector2 direction;
    private SpriteRenderer spriteRenderer;
    public float damage = 10f;
    private bool hasCollided = false;
    private ChargeBar chargeBar;

    public float maxXPos = 10f;
    private float playerInitialX;
    public float slowPercentage = 0f;
    public float slowTimer = 0f;
    public bool isSlowed = false;
    public float originalSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.linearVelocity = direction * speed;
        originalSpeed = speed;
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get SpriteRenderer component
        if (spriteRenderer != null)
        {
            if (direction.x > 0)
            {
                spriteRenderer.flipX = true; 
            }
        }

        if (gameObject.CompareTag("Electron Bullet"))
        {
            playerInitialX = transform.position.x;
        }
        chargeBar = FindObjectOfType<ChargeBar>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSlowed)
        {
            speed = originalSpeed * (1f - slowPercentage);
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0)
            {
                isSlowed = false;
                slowPercentage = 0f;
                speed = originalSpeed;
            }
        }
        else
        {
            speed = originalSpeed;
        }
        
        rb.linearVelocity = direction * speed;

        if (gameObject.CompareTag("Electron Bullet"))
        {
            float distance = Mathf.Abs(transform.position.x - playerInitialX);
            if (distance > maxXPos)
            {
                Destroy(gameObject);
            }
        }
        if ((rb.transform.position.x < -50) || (rb.transform.position.x > 3000))
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hasCollided) return;

        if (hitInfo.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = hitInfo.gameObject.GetComponent<PlayerController>();
            Debug.Log(hitInfo.name);
            playerController.TakeDamage(damage);
            hasCollided = true;
            Destroy(gameObject);
        }
        else if (hitInfo.gameObject.CompareTag("Laser"))
        {
            LaserController laserController = hitInfo.gameObject.GetComponent<LaserController>();
            Debug.Log(hitInfo.name);
            laserController.TakeDamage(damage);
            chargeBar.AddCharge(damage);
            hasCollided = true;
            Destroy(gameObject);
        }
        else if (hitInfo.gameObject.CompareTag("Charger"))
        {
            ChargerController chargerController = hitInfo.gameObject.GetComponent<ChargerController>();
            Debug.Log(hitInfo.name);
            chargerController.TakeDamage(damage);
            chargeBar.AddCharge(damage);
            hasCollided = true;
            Destroy(gameObject);
        }
    }

    public void SlowDown(float percentage, float duration)
    {
        Debug.Log("Bullet Slowed");
        slowPercentage = percentage;
        slowTimer = duration;
        isSlowed = true;
    }
}
