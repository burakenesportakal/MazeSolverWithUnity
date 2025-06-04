using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public Dropdown sizeDropdown;
    public Dropdown solverDropdown;

    public void StartGame()
    {
        // Maze size seçimi
        switch (sizeDropdown.value)
        {
            case 0:
                GameManager.Instance.mazeWidth = 21;
                GameManager.Instance.mazeHeight = 21;
                break;
            case 1:
                GameManager.Instance.mazeWidth = 41;
                GameManager.Instance.mazeHeight = 41;
                break;
            case 2:
                GameManager.Instance.mazeWidth = 61;
                GameManager.Instance.mazeHeight = 61;
                break;
        }

        // Solver algoritması seçimi
        GameManager.Instance.selectedSolver = (SolverType)solverDropdown.value;

        // Maze sahnesine geç
        SceneLoader.nextScene = "MazeScene";
        SceneManager.LoadScene("LoadingScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
