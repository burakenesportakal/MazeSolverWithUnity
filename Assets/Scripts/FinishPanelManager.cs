using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FinishPanelManager : MonoBehaviour
{
    public TMP_Text timeText;
    public GameObject pausePanel;
    private bool isPaused = false;


    public void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ShowPause()
        {
            float finalTime = Timer.Instance.GetElapsedTime(); // artık sabit
            timeText.text = "Time: " + finalTime.ToString("F2") + "s";

            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
