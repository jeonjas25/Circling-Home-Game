using UnityEngine;
using System.Collections.Generic; 

public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 1f;
    public float destroyDelay = 2f;

    private Rigidbody2D rb;
    private Collider2D platformCollider;
    private bool hasBeenTouched = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private static List<FallingPlatform> existingPlatforms = new List<FallingPlatform>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        platformCollider = GetComponent<Collider2D>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        existingPlatforms.Add(this);
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
        //gameObject.SetActive(false);
    }

    void OnDisable() // Called when the GameObject is deactivated
    {
        existingPlatforms.Remove(this);
    }

    void OnEnable() // Called when the GameObject is activated
    {
        if (!existingPlatforms.Contains(this))
        {
            existingPlatforms.Add(this);
        }
    }

    public void resetPlatform()
    {
        hasBeenTouched = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        platformCollider.enabled = true; // Re-enable the collider
        gameObject.SetActive(true); // Reactivate the platform
        CancelInvoke("StartFalling");
    }

     // ResetAllPlatforms is a static function that will call the ResetPlatform function on every falling platform in the scene
    public static void ResetAllPlatforms()
    {
        // Iterate through each FallingPlatform script instance that is stored in the existingPlatforms list
        foreach (FallingPlatform platform in existingPlatforms)
        {
            // Check if the platform reference is not null (it might have been destroyed)
            if (platform != null)
            {
                // Call the ResetPlatform function on this specific falling platform instance
                platform.resetPlatform();
            }
        }
    }
}
