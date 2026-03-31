using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    [SerializeField] int lives = 3;
    public int lastLevelIndex = 1; // last level player played
    int score = 0;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only track actual game levels, skip main menu (0) & GameOver (4)
        if (scene.buildIndex != 0 && scene.buildIndex != 4)
        {
            lastLevelIndex = scene.buildIndex;

            // Optionally play background music for this level
            // AudioManager.Instance?.SetMusicVolume(1f); // or play level music
        }

        // Stop music on GameOver
        // if (scene.buildIndex == 4)
        //     AudioManager.Instance?.SetMusicVolume(0f);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void AddScore(int points)
    {
        score += points;
    }

    public int GetScore() => score;

    public void ProcessPlayerDeath()
    {
        lives--;

        if (lives > 0)
        {
            score = 0;
            RetryLastLevel();
        }
        else
            LoadGameOver();
    }

    public void RetryLastLevel()
    {
        // Reload the last level player was on
        SceneManager.LoadScene(lastLevelIndex);

        // Reset score or other session data if needed
        score = 0;

        // // Optionally restart music
        // AudioManager.Instance?.SetMusicVolume(1f);
    }

    void LoadGameOver()
    {
        Destroy(gameObject); // reset session
        SceneManager.LoadScene(4); // GameOver scene index
        //AudioManager.Instance?.StopMusic();
    }

    public void LoadNextLevel()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
        else
            SceneManager.LoadScene("Win");
    }
}