using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int pointValue = 1;           // How many points this gives
    public float destroyDelay = 0.5f;    // Time to destroy after animation plays

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider2D>().enabled = false;
            // Add points to score
            GameSession.Instance?.AddScore(pointValue);

            // Play animation if Animator exists
            if (animator != null)
            {
                animator.SetTrigger("collect"); // Make sure your Animator has a "Collect" trigger
                AudioManager.Instance.PlayCollect();
            }

            // Destroy the collectible after a short delay
            Destroy(gameObject, destroyDelay);
        }
    }
}