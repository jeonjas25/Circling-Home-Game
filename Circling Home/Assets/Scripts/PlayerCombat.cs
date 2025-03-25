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

    private PlayerController playerController;
    private Animator animator;

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
