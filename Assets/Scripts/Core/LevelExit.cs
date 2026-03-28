using UnityEngine;

public class LevelExit : MonoBehaviour
{
    [SerializeField] float exitDelay = 4f;
    [SerializeField] float hitStopDuration = 0.15f;

    bool levelEnding = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !levelEnding)
        {
            levelEnding = true;

            PlayerMovement movement = other.GetComponent<PlayerMovement>();
            if (movement != null)
                movement.enabled = false;

            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            Animator playerAnim = other.GetComponent<Animator>();
            if (playerAnim != null)
            {
                playerAnim.SetBool("isRunning", false);
                playerAnim.SetBool("isGrounded", true);
                playerAnim.SetFloat("yVelocity", 0);
            }

            // Trigger trophy animation if it has one
            Animator trophyAnim = GetComponent<Animator>();
if (trophyAnim != null)
{
    AudioManager.Instance.PlayWin(); 
    trophyAnim.SetTrigger("celebrate");
    Debug.Log("Trophy animation triggered!");
}
else
{
    Debug.LogError("LevelExit: No Animator found on trophy!");
}

            StartCoroutine(LevelEndSequence());
        }
    }

    System.Collections.IEnumerator LevelEndSequence()
    {
        // Hit stop (freeze game)
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1f;

        // Wait for celebration animation / confetti
        yield return new WaitForSeconds(exitDelay);

        GameSession.Instance?.LoadNextLevel();
    }
}