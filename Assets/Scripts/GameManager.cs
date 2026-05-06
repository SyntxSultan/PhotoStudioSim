using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private PauseMenuUI pauseMenuUI;
    private bool isGamePaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InputManager.SetCursorLock(true);
        pauseMenuUI = FindFirstObjectByType<PauseMenuUI>(FindObjectsInactive.Include);    
    }

    public void TogglePauseGame()
    {
        if (!isGamePaused)
        {
            InputManager.SetCursorLock(false);
            pauseMenuUI.gameObject.SetActive(true);
            Time.timeScale = 0f;
            isGamePaused = true;
        }
        else
        {
            InputManager.SetCursorLock(true);
            pauseMenuUI.gameObject.SetActive(false);
            Time.timeScale = 1f;
            isGamePaused = false;
        }
    }
}
