using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    CapsuleCollider2D touchingCollider;
    Animator animator;
    public ContactFilter2D castFilter;
    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    public float groundDistance = 0.05f;
    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    public float wallDistance = 0.2f;
    RaycastHit2D[] ceilingHits = new RaycastHit2D[5];
    public float ceilingDistance = 0.05f;

    [SerializeField] private bool isGrounded = true;
    public bool IsGrounded { 
        get 
        {
        return isGrounded;
        } 
            private set
        {
            isGrounded = value;
            animator.SetBool("isGrounded", value);
        }  
    }

    [SerializeField] private bool isOnWall;
    public bool IsOnWall { 
        get 
        {
        return isOnWall;
        } 
            private set
        {
            isOnWall = value;
            animator.SetBool("isOnWall", value);
        }  
    }

    [SerializeField] private bool isOnCeiling;
    public bool IsOnCeiling { 
        get 
        {
        return isOnCeiling;
        } 
            private set
        {
            isOnCeiling = value;
            animator.SetBool("isOnCeiling", value);
        }  
    }

    private void Awake()
    {
        touchingCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        IsGrounded = touchingCollider.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
        IsOnWall = touchingCollider.Cast(wallCheckDirection, castFilter, wallHits, wallDistance) > 0;
        IsOnCeiling = touchingCollider.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;
    }
}
