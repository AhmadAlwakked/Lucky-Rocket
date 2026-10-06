using UnityEngine;

public class PausePanel : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pausePanel;

    [Header("Exit Button")]
    public GameObject exitButton;

    [Header("Game Options Menu")]
    public GameObject gameOptionsPanel;

    private bool isPaused = false;

    void Update()
    {
        // ESC = OPEN / CLOSE
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // EXIT BUTTON = OPEN / CLOSE
    public void ExitButton()
    {
        TogglePause();
    }

    // OPEN / CLOSE PAUSE
    public void TogglePause()
    {
        if (isPaused)
        {
            ClosePausePanel();
        }
        else
        {
            OpenPausePanel();
        }
    }

    public void OpenPausePanel()
    {
        isPaused = true;

        pausePanel.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ClosePausePanel()
    {
        isPaused = false;

        pausePanel.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // GAME OPTIONS
    public void OpenGameOptions()
    {
        gameOptionsPanel.SetActive(!gameOptionsPanel.activeSelf);
    }
}