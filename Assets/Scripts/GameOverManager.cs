using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [Tooltip("Drag your Game Over UI Panel here")]
    public GameObject gameOverUI;

    private void OnEnable()
    {
        // Subscribe to the death event
        HealthManager.OnPlayerDeath += ShowGameOver;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        HealthManager.OnPlayerDeath -= ShowGameOver;
    }

    private void ShowGameOver()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);

            Time.timeScale = 0f; 
        }
    }

    public void ResetGame()
    {
        gameOverUI.SetActive(false);
        Time.timeScale = 1f; 
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f; 
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
    }
}