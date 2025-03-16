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

    private PlayerController playerController;
    //private Animator animator;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        //animator = GetComponent<Animator>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("attacked");
            if (isMeleeMode)
            {
                MeleeAttack();
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
            // animator.SetBool("IsMeleeMode", isMeleeMode);
        }
    }

    void MeleeAttack()
    {

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
}
