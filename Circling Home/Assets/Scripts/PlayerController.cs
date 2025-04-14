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

    private bool isJumping = false;
    private float verticalVelocity = 0f;
    public float initialJumpForce = 10f;
    public float gravityStrength = 20f;
    public float fallGravityMultiplier = 2.5f;
    private bool isJumpButtonHeld = false;
    public float jumpCutOffMultiplier = 0.5f;
    public float maxJumpVelocity = 30f;

    private bool canUseCoyote = false;
    private float lastTimeGrounded;
    public float coyoteTimeDuration = 0.15f;
    private bool wasGroundedLastFrame = false;

    private bool jumpInputBuffered = false;
    private float jumpBufferTimeCounter;
    public float jumpBufferDuration = 0.2f;

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

        if (touchingDirections.IsGrounded)
        {
            lastTimeGrounded = Time.time;
            canUseCoyote = true;
        }

            // Handle jump buffering
        if (jumpInputBuffered)
        {
            jumpBufferTimeCounter -= Time.deltaTime;
            if (jumpBufferTimeCounter <= 0)
            {
                jumpInputBuffered = false; // Buffer timed out
            }
        }

        // Check if we just landed AND we have a buffered jump
        if (touchingDirections.IsGrounded && !wasGroundedLastFrame && jumpInputBuffered)
        {
            // Trigger the jump!
            isJumping = true;
            verticalVelocity = initialJumpForce;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            isJumpButtonHeld = true;
            jumpInputBuffered = false; // Clear the buffer after using it
            Debug.Log("Buffer Jump Triggered");
        }

        wasGroundedLastFrame = touchingDirections.IsGrounded;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.linearVelocity.y);
        
        if (isJumping)
        {
            if (verticalVelocity > 0 && isJumpButtonHeld)
            {
                // While holding and going up, reduce gravity's effect
                verticalVelocity -= (gravityStrength * (1f - jumpCutOffMultiplier)) * Time.fixedDeltaTime;
                verticalVelocity = Mathf.Min(verticalVelocity, maxJumpVelocity);
            }
            else if (verticalVelocity > 0 && !isJumpButtonHeld)
            {
                // If button released while going up, start falling sooner
                verticalVelocity -= gravityStrength * fallGravityMultiplier * Time.fixedDeltaTime;
            }
            else
            {
                // Falling
                verticalVelocity -= gravityStrength * fallGravityMultiplier * Time.fixedDeltaTime;
            }

            transform.Translate(Vector3.up * verticalVelocity * Time.fixedDeltaTime);

            // Check if grounded
            if (touchingDirections.IsGrounded && verticalVelocity <= 0)
            {
                isJumping = false;
                verticalVelocity = 0f;
            }
        }
        else if (touchingDirections.IsGrounded)
        {
            verticalVelocity = Mathf.Min(0f, verticalVelocity);
        }
        else
        {
            verticalVelocity -= gravityStrength * fallGravityMultiplier * Time.fixedDeltaTime;
            transform.Translate(Vector3.up * verticalVelocity * Time.fixedDeltaTime);
        }
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
        if (context.started)
        {
            if (touchingDirections.IsGrounded || CanUseCoyoteTime())
            {
                isJumping = true;
                verticalVelocity = initialJumpForce;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                isJumpButtonHeld = true;
                if (CanUseCoyoteTime())
                {
                    canUseCoyote = false;
                    Debug.Log("Coyote Time Used for Jump");
                }
                jumpInputBuffered = false;
            }
            else 
            {
                jumpInputBuffered = true;
                jumpBufferTimeCounter = jumpBufferDuration;
            }
        }

        if (context.canceled && isJumping)
        {
            isJumpButtonHeld = false;
        }
    }

    private bool CanUseCoyoteTime()
    {
        return canUseCoyote && Time.time < lastTimeGrounded + coyoteTimeDuration && !touchingDirections.IsGrounded;
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
