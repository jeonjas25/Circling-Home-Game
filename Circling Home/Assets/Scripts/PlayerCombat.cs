using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    public bool isMeleeMode = true;
    public Transform RangedAttackPoint;
    public float attackRange = 0.5f;
    public float fireRate = 0.5f;
    public float nextFireTime = 0f;
    public LayerMask enemyLayers;
    public GameObject bulletPrefab;
    public GameObject lightningPrefab;
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
    public TouchingDirections touchingDirections;
    private bool isSuperCharging = false;
    private float superAttackTimer = 0f;
    public float superAttackHoldDuration = 1f;
    private AudioManager audioManager;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
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

    /*public void OnSuper(InputAction.CallbackContext context)
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
                }
            }
        }
    }*/

    void MeleeAttack()
    {
        animator.SetTrigger("Slash");
        audioManager.PlaySFX(audioManager.melee);

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
            else if (enemy.CompareTag("Charger"))
            {
                ChargerController chargerController = enemy.GetComponent<ChargerController>();
                if (chargerController != null)
                {
                    chargerController.TakeDamage(meleeAttackDamage);
                    chargeBar.AddCharge(meleeAttackDamage);
                }
            }
        }
    }

    void RangedAttack()
    {
        if (Time.time >= nextFireTime)
        {
            audioManager.PlaySFX(audioManager.laser);
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
        if (Time.time >= nextRushTime && !isRushing)
        {
            StartCoroutine(SwordRush());
            chargeBar.ResetCharge();
            nextRushTime = Time.time + rushCooldown;
        }
    }

    void RangedSuperAttack()
    {
        Debug.Log("Ranged Super Attack");
        if (Time.time >= nextFireTime)
        {
            GameObject lightningBolt = Instantiate(lightningPrefab, RangedAttackPoint.position, Quaternion.identity);
            Debug.Log("Lightning bolt instantiated");
            audioManager.PlaySFX(audioManager.lightningBolt);
            ChainLightningBoltController chainLightningBoltController = lightningBolt.GetComponent<ChainLightningBoltController>();

            if (playerController.isMovingRight) // or however you are tracking player direction.
            {
                chainLightningBoltController.direction = Vector2.right; // Fire right
            }
            else if (playerController.isMovingLeft)
            {
                chainLightningBoltController.direction = Vector2.left; // Fire left
            }
            else
            {
                chainLightningBoltController.direction = Vector2.right; // default to right, or whatever default direction you want.
            }

            nextFireTime = Time.time + fireRate;
        }
        chargeBar.ResetCharge();
    }

    IEnumerator SwordRush()
    {
        Debug.Log("Sword Rush Triggered");
        audioManager.PlaySFX(audioManager.slash);
        isRushing = true;
        originalLayer = gameObject.layer;
        gameObject.layer = Mathf.RoundToInt(Mathf.Log(rushingLayer.value, 2));

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Laser"); // Find all enemies with the "Laser" tag
        Collider2D playerCollider = GetComponent<Collider2D>(); // Get the player's main collider

        foreach (GameObject enemy in enemies)
        {
            Collider2D enemyCollider = enemy.GetComponent<Collider2D>(); // Get the enemy's collider
            Physics2D.IgnoreCollision(playerCollider, enemyCollider, true); // Ignore collisions
        }

        animator.SetBool("isRushing", true);

        float rushDirection = 1f; // Default to right

        if (playerController.isMovingRight)
        {
            rushDirection = 1f; // Right
        }
        else if (playerController.isMovingLeft)
        {
            rushDirection = -1f; // Left
        }

        float startTime = Time.time;
        List<Collider2D> damagedEnemies = new List<Collider2D>();

        while (Time.time < startTime + rushDuration)
        {
            if (touchingDirections.IsGrounded)
            {
                Vector2 raycastPosition = (Vector2) transform.position + new Vector2(raycastOffset.x * rushDirection, raycastOffset.y);
                RaycastHit2D hit = Physics2D.Raycast(raycastPosition, Vector2.down, raycastDistance, groundLayer);
                Debug.DrawRay(raycastPosition, Vector2.down * raycastDistance, Color.red);

                if (hit.collider == null) 
                {
                    break;
                }
            }

            float airRushSpeed = rushSpeed / 2; // Half the speed in the air
            float airRushDistance = rushDistance / 2; // Half the distance in the air
            float downwardForce = 5f; // Add a downward force

            Vector2 rushEndPosition = (Vector2)transform.position + Vector2.right * rushDirection * (touchingDirections.IsGrounded ? rushDistance : airRushDistance);

            Collider2D[] hitEnemies = Physics2D.OverlapAreaAll(
                new Vector2(Mathf.Min(transform.position.x, rushEndPosition.x), Mathf.Min(transform.position.y, rushEndPosition.y)),
                new Vector2(Mathf.Max(transform.position.x, rushEndPosition.x), Mathf.Max(transform.position.y, rushEndPosition.y)),
                enemyLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.CompareTag("Laser"))
                {
                    if (!damagedEnemies.Contains(enemy)) // Check if already damaged
                    {
                        LaserController laserController = enemy.GetComponent<LaserController>();
                        if (laserController != null)
                        {
                            laserController.TakeDamage(rushDamage);
                            damagedEnemies.Add(enemy); // Add to the list
                        }
                    }
                }
                else if (enemy.CompareTag("Charger"))
                {
                    if (!damagedEnemies.Contains(enemy))
                    {
                        ChargerController chargerController = enemy.GetComponent<ChargerController>();
                        if (chargerController != null)
                        {
                            chargerController.TakeDamage(rushDamage);
                            damagedEnemies.Add(enemy);
                        }
                    }
                }
            }

            Vector2 movement = Vector2.MoveTowards(transform.position, rushEndPosition, (touchingDirections.IsGrounded ? rushSpeed : airRushSpeed) * Time.deltaTime);

            if (!touchingDirections.IsGrounded){
                movement += Vector2.down * downwardForce * Time.deltaTime;
            }

            transform.position = movement;

            yield return null;
        }

        animator.SetBool("isRushing", false);
        gameObject.layer = originalLayer; // Restore original layer
        isRushing = false;

        foreach (GameObject enemy in enemies)
        {
            Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(playerCollider, enemyCollider, false); // Re-enable collisions
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        // Check if the J key is held down
        if (Input.GetKey(KeyCode.J))
        {
            if (!isSuperCharging)
            {
                isSuperCharging = true;
                superAttackTimer = 0f; // Start the timer when J is first pressed
            }

            if (isSuperCharging)
            {
                superAttackTimer += Time.deltaTime; // Increment timer

                if (chargeBar.IsChargeFull() && superAttackTimer >= superAttackHoldDuration) //Check timer
                {
                    //  IMPORTANT:  Only trigger ONCE.
                    if (isMeleeMode)
                    {
                        MeleeSuperAttack();
                    }
                    else
                    {
                        RangedSuperAttack();
                    }
                    isSuperCharging = false; // Reset so it doesn't repeat.
                    superAttackTimer = 0f;
                }
            }
        }
        else if (isSuperCharging)
        {
            isSuperCharging = false;
            superAttackTimer = 0f;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (meleeAttackPoint != null)
        {
            Gizmos.DrawWireSphere(meleeAttackPoint.position, meleeAttackRange);
        }
    }
}
