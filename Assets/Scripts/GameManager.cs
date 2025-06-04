using UnityEngine;

public enum SolverType { DFS, BFS, Dijkstra, AStar, Manual }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public SolverType selectedSolver;
    public int mazeWidth = 21;
    public int mazeHeight = 21;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Scene geçişlerinde kalıcı
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMazeSize(int size)
    {
        mazeWidth = size;
        mazeHeight = size;
    }

    public void SetSolverType(int typeIndex)
    {
        selectedSolver = (SolverType)typeIndex;
    }
}
