using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    public bool isMeleeMode = true;
    public Transform RangedAttackPoint;
    public float attackRange = 0.5f;
    public float fireRate = 0.5f;
    public float nextFireTime = 0f;
    public LayerMask enemyLayers;
    public GameObject bulletPrefab;
    public Collider2D playerCollider;
    public Transform meleeAttackPoint;
    public float meleeAttackRange = 0.5f;
    public float meleeAttackDamage = 20f;
    public float meleeAttackCooldown = 1f;
    private float nextAttackTime = 0f;
    public ChargeBar chargeBar;
    private PlayerController playerController;
    private Animator animator;
    public float rushDistance = 10f;
    public float rushSpeed = 20f;
    public float rushDuration = 2f;
    public float rushDamage = 40f;
    public float rushCooldown = 2f;
    public LayerMask rushingLayer;
    public LayerMask groundLayer;
    private int originalLayer;
    private bool isRushing = false;
    private float nextRushTime = 0f;
    public Vector2 raycastOffset = new Vector2(0.24f, -0.75f);
    public float raycastDistance = 1.5f;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("attacked");
            if (isMeleeMode)
            {
                if (Time.time >= nextAttackTime)
                {
                    MeleeAttack();
                    nextAttackTime = Time.time + meleeAttackCooldown;
                }
            }
            else 
            {
                RangedAttack();
            }
        }
    }

    public void OnSwitch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isMeleeMode = !isMeleeMode;
            Debug.Log("Mode switched to: " + (isMeleeMode ? "Melee" : "Ranged"));
            animator.SetBool("IsMeleeMode", isMeleeMode);
        }
    }

    public void OnSuper(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (chargeBar.IsChargeFull())
            {
                Debug.Log("Charge Attack");
                if (isMeleeMode)
                {
                    MeleeSuperAttack();
                }
                else 
                {
                    RangedSuperAttack();
                    chargeBar.ResetCharge();
                }
            }
        }
    }
    void MeleeAttack()
    {
        animator.SetTrigger("Slash");

        if (playerController.isMovingRight)
        {
            meleeAttackPoint.localPosition = new Vector3(Mathf.Abs(meleeAttackPoint.localPosition.x), meleeAttackPoint.localPosition.y, meleeAttackPoint.localPosition.z);
        }
        else if (playerController.isMovingLeft)
        {
            meleeAttackPoint.localPosition = new Vector3(-Mathf.Abs(meleeAttackPoint.localPosition.x), meleeAttackPoint.localPosition.y, meleeAttackPoint.localPosition.z);
        }

         // Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(meleeAttackPoint.position, meleeAttackRange, enemyLayers);

        // Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            if (enemy.CompareTag("Laser"))
            {
                LaserController laserController = enemy.GetComponent<LaserController>();
                if (laserController != null)
                {
                    laserController.TakeDamage(meleeAttackDamage);
                    chargeBar.AddCharge(meleeAttackDamage);
                }
            }
        }
    }

    void RangedAttack()
    {
        if (Time.time >= nextFireTime)
        {
            GameObject bullet = Instantiate(bulletPrefab, RangedAttackPoint.position, Quaternion.identity);
            LaserBulletController laserBulletController = bullet.GetComponent<LaserBulletController>();

            if (playerController.isMovingRight) // or however you are tracking player direction.
            {
                laserBulletController.direction = Vector2.right; // Fire right
            }
            else if (playerController.isMovingLeft)
            {
                laserBulletController.direction = Vector2.left; // Fire left
            }
            else
            {
                laserBulletController.direction = Vector2.right; // default to right, or whatever default direction you want.
            }

            // animator.SetTrigger("Ranged Attack");
            nextFireTime = Time.time + fireRate;
        }
    }

    void MeleeSuperAttack()
    {
        if (Time.time >= nextRushTime && !isRushing) // Check cooldown and not dashing
        {
            StartCoroutine(SwordRush());
            chargeBar.ResetCharge();
            nextRushTime = Time.time + rushCooldown;
        }
    }

    void RangedSuperAttack()
    {

    }

    IEnumerator SwordRush()
    {
        Debug.Log("Sword Rush Triggered");
        isRushing = true;
        originalLayer = gameObject.layer;
        gameObject.layer = Mathf.RoundToInt(Mathf.Log(rushingLayer.value, 2));

        //animator.SetTrigger("Rush");

        float rushDirection = playerController.isMovingRight ? 1f : -1f;

        float startTime = Time.time;

        while (Time.time < startTime + rushDuration)
        {
            Vector2 raycastPosition = (Vector2) transform.position + new Vector2(raycastOffset.x * rushDirection, raycastOffset.y);
            RaycastHit2D hit = Physics2D.Raycast(raycastPosition, Vector2.down, raycastDistance, groundLayer);
            Debug.DrawRay(raycastPosition, Vector2.down * raycastDistance, Color.red);

            if (hit.collider == null) 
            {
                break;
            }

            Vector2 rushEndPosition = (Vector2)transform.position + Vector2.right * rushDirection * rushDistance;
            transform.position = Vector2.MoveTowards(transform.position, rushEndPosition, rushSpeed * Time.deltaTime);

            yield return null;
        }

        gameObject.layer = originalLayer; // Restore original layer
        isRushing = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmosSelected()
    {
        if (meleeAttackPoint != null)
        {
            Gizmos.DrawWireSphere(meleeAttackPoint.position, meleeAttackRange);
        }
    }
}
