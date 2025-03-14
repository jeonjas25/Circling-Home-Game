using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    Vector2 moveInput;
    public float walkSpeed;
    public float jumpImpulse;
    public float airWalkSpeed;
    public float maxHealth = 100f;
    public float currentHealth;
    public float CurrentMoveSpeed {
        get
        {
            if (IsMoving && !touchingDirections.IsOnWall)
            {
                if (touchingDirections.IsGrounded)
                {
                    return walkSpeed;
                }
                else 
                {
                    return airWalkSpeed;
                }
            }
            else 
            {
                return 0;
            }
        }
    }
    public bool IsMoving { 
        get 
        {
            return isMoving;
        } 
        private set 
        {
            isMoving = value;
            animator.SetBool("isMoving", value);
        }
    }
    public bool IsMovingLeft { 
        get 
        {
            return isMovingLeft;
        } 
        private set 
        {
            isMovingLeft = value;
            animator.SetBool("isMovingLeft", value);
        }
    }
    public bool IsMovingRight { 
        get 
        {
            return isMovingRight;
        } 
        private set 
        {
            isMovingRight = value;
            animator.SetBool("isMovingRight", value);
        }
    }
    private bool isMoving = false;
    private bool isMovingLeft = false;
    private bool isMovingRight = false;
    private Rigidbody2D rb;
    Animator animator;
    TouchingDirections touchingDirections;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.linearVelocity.y);
        Debug.Log(currentHealth);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        IsMoving = moveInput != Vector2.zero;

        if (IsMoving)
        {
            IsMovingRight = moveInput.x > 0;
            IsMovingLeft = moveInput.x < 0;
        }
        else
        {
            IsMovingLeft = false;
            IsMovingRight = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // todo check if alive
        if (context.started && touchingDirections.IsGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void HealDamage(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void Die()
    {
        if (currentHealth <= 0f)
        {
            Debug.Log("Player Died!");
        }
    }
}
