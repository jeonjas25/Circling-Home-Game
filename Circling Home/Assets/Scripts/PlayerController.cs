using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    Vector2 moveInput;
    public float walkSpeed;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float doubleTapThreshold = 0.3f;
    public float jumpImpulse;
    public float airWalkSpeed;
    public float maxHealth = 100f;
    public float currentHealth;
    public Vector2 respawnPosition;
    public HealthBar healthBar;
    public ChargeBar chargeBar;
    public float CurrentMoveSpeed
    {
        get
        {
            if (isDashing)
            {
                return dashSpeed;
            }
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
    public bool isMoving = false;
    public bool isMovingLeft = false;
    public bool isMovingRight = false;
    private Rigidbody2D rb;
    Animator animator;
    TouchingDirections touchingDirections;
    private float lastMoveTime;
    private float lastMoveDirection;
    private float dashTimer;
    private bool isDashing;
    public float xLimitLeft = -16f;

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
        respawnPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -8)
        {
            Die();
        }

        if (isDashing)
        {
            dashTimer += Time.deltaTime;
            if (dashTimer >= dashDuration)
            {
                isDashing = false;
            }
        }

        if (transform.position.x < xLimitLeft)
        {
            transform.position = new Vector3(xLimitLeft, transform.position.y, transform.position.z);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.linearVelocity.y);
        //Debug.Log(currentHealth);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        IsMoving = moveInput != Vector2.zero;

        if (IsMoving)
        {
            IsMovingRight = moveInput.x > 0;
            IsMovingLeft = moveInput.x < 0;

            if (context.started)
            {
                CheckForDoubleTap(moveInput.x);
            }
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

    void CheckForDoubleTap(float moveDirection)
    {
        float currentTime = Time.time;

        if (Mathf.Sign(moveDirection) == Mathf.Sign(lastMoveDirection) && currentTime - lastMoveTime < doubleTapThreshold)
        {
            Dash(moveDirection);
        }

        lastMoveTime = currentTime;
        lastMoveDirection = moveDirection;
    }

    void Dash(float direction)
    {
        isDashing = true;
        dashTimer = 0f;
        moveInput.x = direction;
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
        Debug.Log("Player Died!");
        Respawn();
    }

    public void Respawn()
    {
        transform.position = respawnPosition;
        transform.rotation = Quaternion.identity;
        currentHealth = maxHealth;
        healthBar.Reset();
        chargeBar.ResetCharge();
    }
}
