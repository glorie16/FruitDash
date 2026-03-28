using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] float speed = 15f;
    [SerializeField] float maxLifetime = 3f;
    [SerializeField] int damage = 1;
    [SerializeField] LayerMask hitLayers;

    SpriteRenderer spriteRenderer;
    float direction = 1f;
    Vector2 previousPosition;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        previousPosition = transform.position;
        Destroy(gameObject, maxLifetime);
    }

    public void SetDirection(float dir)
    {
        direction = Mathf.Sign(dir);

        if (spriteRenderer != null)
            spriteRenderer.flipX = direction < 0;
    }

    void Update()
    {
        Vector2 currentPosition = transform.position;
        Vector2 move = Vector2.right * direction * speed * Time.deltaTime;
        Vector2 nextPosition = currentPosition + move;

        RaycastHit2D hit = Physics2D.Linecast(currentPosition, nextPosition, hitLayers);

        Debug.DrawLine(currentPosition, nextPosition, Color.red);

        if (hit.collider != null)
        {
            Debug.Log("Arrow hit: " + hit.collider.name + " | Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));

            EnemyPatrol enemy = hit.collider.GetComponentInParent<EnemyPatrol>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage, transform.position);
                CameraShake.Instance?.Shake(1f, 0.15f);
                Destroy(gameObject);
                return;
            }

            Destroy(gameObject);
            return;
        }

        transform.position = nextPosition;
        previousPosition = currentPosition;
    }
}