using UnityEngine;

public class LaserBulletController : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;
    public Vector2 direction;
    private SpriteRenderer spriteRenderer;
    public float damage = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.linearVelocity = direction * speed;
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get SpriteRenderer component
        if (spriteRenderer != null)
        {
            if (direction.x > 0)
            {
                spriteRenderer.flipX = true; 
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((rb.transform.position.x < -50) || (rb.transform.position.x > 3000))
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = hitInfo.gameObject.GetComponent<PlayerController>();
            Debug.Log(hitInfo.name);
            playerController.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
