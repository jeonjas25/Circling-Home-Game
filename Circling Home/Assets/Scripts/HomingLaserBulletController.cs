using UnityEngine;

public class HomingLaserBulletController : MonoBehaviour
{
    public float homingSpeed = 8f;
    public float rotationSpeed = 100f;
    public float initialSpeed = 20f;
    public float homingDuration = 2f;
    public float lifeTime = 3f;
    public GameObject explosionPrefab;
    private Transform target;
    public Rigidbody2D rb;
    private float homingTimer = 0f;
    private float lifeTimer = 0f;
    private bool isHoming = true;
    private bool hasCollided = false;
    public Vector2 direction;
    public float damage = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindPlayer();
        rb.linearVelocity = direction * initialSpeed;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        lifeTimer += Time.fixedDeltaTime;

        if (isHoming && target != null)
        {
            homingTimer += Time.fixedDeltaTime;
            Vector2 directionToPlayer = (Vector2) target.position - rb.position;
            directionToPlayer.Normalize();
            directionToPlayer += Random.insideUnitCircle * 0.5f;
            directionToPlayer.Normalize();

            float rotateAmount = Vector3.Cross(directionToPlayer, transform.right).z;
            rb.angularVelocity = -rotateAmount * rotationSpeed;
            rb.linearVelocity = transform.right * homingSpeed;

            if (homingTimer >= homingDuration)
            {
                isHoming = false;
                rb.angularVelocity = 0;
            }
        }

        if (lifeTimer >= lifeTime)
        {
            Explode();
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

    void Explode()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
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
    }
}
