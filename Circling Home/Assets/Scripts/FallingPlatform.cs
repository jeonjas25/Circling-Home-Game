using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 1f;
    public float destroyDelay = 2f;

    private Rigidbody2D rb;
    private Collider2D platformCollider;
    private bool hasBeenTouched = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        platformCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object that collided with us has the "Player" tag
        if (collision.gameObject.CompareTag("Player") && !hasBeenTouched)
        {
            // Mark that the platform has been touched
            hasBeenTouched = true;

            // Start the falling process after a delay
            Invoke("StartFalling", fallDelay);
        }
    }

    void StartFalling()
    {
        // Change the Rigidbody 2D to Dynamic so it's affected by gravity
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Disable the platform's collider so the player falls with it (optional)
        // platformCollider.enabled = false;

        // Destroy the platform after a delay
        Destroy(gameObject, destroyDelay);
    }
}
