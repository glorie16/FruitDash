using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float jumpForce = 14f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.15f;
    [SerializeField] LayerMask groundLayer;

    [Header("Jump Feel")]
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2f;

    [Header("Jump Settings")]
    [SerializeField] int maxJumps = 2;

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;

    bool isGrounded;
    bool isAlive = true;
    float horizontalInput;

    int jumpCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Time.timeScale == 0) return; 
        if (!isAlive) return;

        ReadInput();
        CheckGrounded();
        HandleJump();
        FlipSprite();
        UpdateAnimator();
    }

    // ─── Debug Collisions ─────────────────────────
void OnCollisionEnter2D(Collision2D collision)
{
    //Debug.Log($"[Collision] Player touched: {collision.gameObject.name} | Tag: {collision.gameObject.tag}");
}

void OnTriggerEnter2D(Collider2D other)
{
    //Debug.Log($"[Trigger] Player entered trigger: {other.gameObject.name} | Tag: {other.gameObject.tag}");
}

    void FixedUpdate()
    {
        if (!isAlive) return;

        Move();
        ApplyBetterJumpPhysics();
    }

    // ─── Input ─────────────────────────
    void ReadInput()
    {
        horizontalInput = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            horizontalInput = -1f;

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            horizontalInput = 1f;
    }

    // ─── Movement ──────────────────────
    void Move()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    // ─── Ground Check ──────────────────
    void CheckGrounded()
    {
      bool wasGrounded = isGrounded;

    Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

    if (hit != null)
    {
        //Debug.Log($"Player standing on: {hit.gameObject.name} | Tag: {hit.gameObject.tag}");
    }

    isGrounded = hit != null; // update grounded state

    if (!wasGrounded && isGrounded)
    {
        // Reset jump count on landing
        jumpCount = 0;
    }
    }

    // ─── Jump ──────────────────────────
    void HandleJump()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && jumpCount < maxJumps)
        {
            // Reset vertical velocity for consistent jump force
            AudioManager.Instance.PlayJump();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            jumpCount++;

            if (jumpCount == 1)
            {
                // First jump (normal)
                animator.SetBool("isGrounded", false);
            }
            else if (jumpCount == 2)
            {
                // Second jump (double jump)
                animator.SetTrigger("doubleJump");
            }
        }
    }

    // ─── Better Jump ───────────────────
    void ApplyBetterJumpPhysics()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Keyboard.current.spaceKey.isPressed)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    // ─── Sprite Flip ───────────────────
    void FlipSprite()
    {
        if (horizontalInput > 0)
            spriteRenderer.flipX = false;
        else if (horizontalInput < 0)
            spriteRenderer.flipX = true;
    }

    // ─── Animator ──────────────────────
    void UpdateAnimator()
    {   
        bool isRunning = Mathf.Abs(horizontalInput) > Mathf.Epsilon;

        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    public void OnDeath()
    {
        isAlive = false;
        animator.SetTrigger("die");
        AudioManager.Instance.PlayDeath();
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
    }
}

