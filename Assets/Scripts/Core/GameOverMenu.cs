using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public void RetryLevel()
    {
        GameSession.Instance?.RetryLastLevel();
    }

    public void QuitToMenu()
    {
        // Load the main menu (scene index 0)
        SceneManager.LoadScene(0);
    }
}