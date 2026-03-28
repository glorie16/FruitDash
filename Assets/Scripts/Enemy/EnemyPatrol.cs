using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] Transform leftEdge;
    [SerializeField] Transform rightEdge;

    [Header("Ground Detection")]
    [SerializeField] Transform groundDetect;
    [SerializeField] float detectRadius = 0.2f;
    [SerializeField] float frontCheckDistance = 0.35f;
    [SerializeField] LayerMask groundLayer;

    [Header("Turn Delay")]
    [SerializeField] float turnDelay = 0.5f;

    [Header("Health")]
    [SerializeField] int maxHealth = 3;

    [Header("Death Pop")]
    [SerializeField] float deathPopY = 7f;
    [SerializeField] float deathPopX = 2f;
    [SerializeField] float deathGravity = 6f;
    [SerializeField] float lethalHurtDelay = 0.15f;
    [SerializeField] float deathDestroyDelay = 2f;

    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Animator animator;
    Collider2D enemyCollider;

    bool movingRight = true;
    bool isAlive = true;
    bool isTurning = false;
    bool isDying = false;

    float horizontalInput;
    int currentHealth;

    // Stores which way to knock the enemy on death
    float deathKnockbackDirection = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();

        currentHealth = maxHealth;
    }

    void Update()
    {
        if (!isAlive || isDying) return;

        UpdateMovementState();
        CheckEdges();
        FlipSprite();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (!isAlive || isDying) return;

        Move();
    }

    void UpdateMovementState()
    {
        horizontalInput = isTurning ? 0f : (movingRight ? 1f : -1f);
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    void CheckEdges()
    {
        if (isTurning) return;

        if (movingRight && transform.position.x >= rightEdge.position.x)
        {
            StartCoroutine(TurnAroundWithDelay());
            return;
        }

        if (!movingRight && transform.position.x <= leftEdge.position.x)
        {
            StartCoroutine(TurnAroundWithDelay());
            return;
        }

        Vector2 checkPos = (Vector2)groundDetect.position +
                           new Vector2(movingRight ? frontCheckDistance : -frontCheckDistance, 0f);

        bool groundAhead = Physics2D.OverlapCircle(checkPos, detectRadius, groundLayer);

        if (!groundAhead)
        {
            StartCoroutine(TurnAroundWithDelay());
        }
    }

    IEnumerator TurnAroundWithDelay()
    {
        if (isTurning || isDying) yield break;

        isTurning = true;
        horizontalInput = 0f;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        yield return new WaitForSeconds(turnDelay);

        movingRight = !movingRight;
        isTurning = false;
    }

    void FlipSprite()
    {
        if (horizontalInput > 0)
            spriteRenderer.flipX = true;   // enemy art faces left by default
        else if (horizontalInput < 0)
            spriteRenderer.flipX = false;
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        bool isRunning = Mathf.Abs(horizontalInput) > Mathf.Epsilon;
        animator.SetBool("isRunning", isRunning);
    }

    public void TakeDamage(int damage, Vector2 hitSourcePosition)
    {
        if (!isAlive || isDying) return;

        currentHealth -= damage;

        // Knock away from the hit source
        deathKnockbackDirection = transform.position.x >= hitSourcePosition.x ? 1f : -1f;

        if (currentHealth <= 0)
        {
            if (animator != null) {
                animator.SetTrigger("hurt");
            }
            
            AudioManager.Instance.PlayFinish();

            StopAllCoroutines();
            StartCoroutine(DieAfterHurt());
            return;
        }

        if (animator != null)
            animator.SetTrigger("hurt");
            AudioManager.Instance.PlayHit();
    }

    // Optional overload so old calls still work
    public void TakeDamage(int damage)
    {
        TakeDamage(damage, transform.position);
    }

    IEnumerator DieAfterHurt()
    {
        isAlive = false;
        isDying = true;
        isTurning = false;
        horizontalInput = 0f;

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(lethalHurtDelay);

        rb.linearVelocity = new Vector2(deathKnockbackDirection * deathPopX, deathPopY);
        rb.gravityScale = deathGravity;

        if (enemyCollider != null)
            enemyCollider.enabled = false;

        Destroy(gameObject, deathDestroyDelay);
    }

    void OnDrawGizmosSelected()
    {
        if (groundDetect != null)
        {
            Vector2 checkPos = (Vector2)groundDetect.position +
                               new Vector2(movingRight ? frontCheckDistance : -frontCheckDistance, 0f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(checkPos, detectRadius);
        }
    }
}