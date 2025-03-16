using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public bool isMeleeMode = true;
    // public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

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
