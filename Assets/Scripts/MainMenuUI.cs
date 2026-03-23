using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public Dropdown sizeDropdown;
    public Dropdown solverDropdown;

    public void StartGame()
    {
        //choose the size of maze
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

        //choose the solve algorithm
        GameManager.Instance.selectedSolver = (SolverType)solverDropdown.value;

        SceneLoader.nextScene = "MazeScene";
        SceneManager.LoadScene("LoadingScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
