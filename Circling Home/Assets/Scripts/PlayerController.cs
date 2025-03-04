using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    Vector2 moveInput;
    public float walkSpeed;
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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * walkSpeed, rb.linearVelocity.y);
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
}
