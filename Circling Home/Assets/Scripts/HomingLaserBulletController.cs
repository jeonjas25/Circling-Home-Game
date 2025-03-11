using UnityEngine;

public class HomingLaserBulletController : MonoBehaviour
{
    public float homingSpeed = 5f;
    public float rotationSpeed = 200f;
    private Transform target;
    public Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindPlayer();
        rb.linearVelocity = transform.right * homingSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null)
        {
            Vector2 direction = (Vector2)target.position - rb.position;
            direction.Normalize();
            float rotateAmount = Vector3.Cross(direction, transform.right).z;
            rb.angularVelocity = -rotateAmount * rotationSpeed;
            rb.linearVelocity = transform.right * homingSpeed;
        }
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }
}
