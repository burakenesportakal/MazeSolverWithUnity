using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    int width;
    int height;

    public Vector3 GoalWorldPosition { get; private set; }

    [Header("Maze Prefabs")]
    public GameObject wallPrefab;
    public GameObject pathPrefab;
    public GameObject startFlagPrefab;
    public GameObject goalFlagPrefab;
    public GameObject playerPrefab;
    public Transform mazeParent;


    public int[,] maze;
    public GameObject[,] tiles;

    [Header("Camera Settings")]
    public Camera mainCamera;

    [System.Serializable]
    public struct CameraSettings
    {
        public Vector3 position;
        public Vector3 eulerAngles;
    }

    public CameraSettings smallCamSettings;
    public CameraSettings mediumCamSettings;
    public CameraSettings bigCamSettings;

    void Start()
    {
        width = GameManager.Instance.mazeWidth;
        height = GameManager.Instance.mazeHeight;

        SetCameraBySize(width);
        ClearMaze();
        GenerateMaze();
        SpawnMaze();

        FindObjectOfType<MazeSolver>().Initialize(maze, tiles);
    }

    void SetCameraBySize(int size)
    {
        if (size <= 21)
            SetCameraPosition(smallCamSettings);
        else if (size <= 41)
            SetCameraPosition(mediumCamSettings);
        else
            SetCameraPosition(bigCamSettings);
    }

    void SetCameraPosition(CameraSettings settings)
    {
        mainCamera.transform.position = settings.position;
        mainCamera.transform.rotation = Quaternion.Euler(settings.eulerAngles);
    }

    public void ClearMaze()
    {
        if (mazeParent == null)
        {
            Debug.LogWarning("mazeParent reference not assigned!");
            return;
        }

        for (int i = mazeParent.childCount - 1; i >= 0; i--)
        {
            Destroy(mazeParent.GetChild(i).gameObject);
        }
    }

    void GenerateMaze()
    {
        maze = new int[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                maze[x, y] = 1;

        Carve(1, 1);
    }

    void Carve(int x, int y)
    {
        maze[x, y] = 0;

        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        dirs = Shuffle(dirs);

        foreach (var dir in dirs)
        {
            int nx = x + dir.x * 2;
            int ny = y + dir.y * 2;

            if (nx > 0 && ny > 0 && nx < width - 1 && ny < height - 1 && maze[nx, ny] == 1)
            {
                maze[x + dir.x, y + dir.y] = 0;
                Carve(nx, ny);
            }
        }
    }

    Vector2Int[] Shuffle(Vector2Int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int r = Random.Range(i, array.Length);
            (array[i], array[r]) = (array[r], array[i]);
        }
        return array;
    }

    void SpawnMaze()
    {
        tiles = new GameObject[width, height];
        float spacing = 1f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject prefab = (maze[x, y] == 1) ? wallPrefab : pathPrefab;
                Vector3 pos = new Vector3(x * spacing, 0, y * spacing);
                tiles[x, y] = Instantiate(prefab, pos, Quaternion.identity, mazeParent);
            }
        }

        Vector3 startPos = new Vector3(1 * spacing, 0.5f, 1 * spacing);
        Vector3 goalPos = new Vector3((width - 2) * spacing, 0.5f, (height - 2) * spacing);

        Instantiate(startFlagPrefab, startPos, Quaternion.identity, mazeParent);

        GameObject goal = Instantiate(goalFlagPrefab, goalPos, Quaternion.identity, mazeParent);
        goal.tag = "Goal"; // Manuel çözümde tespiti için

        if (GameManager.Instance.selectedSolver == SolverType.Manual)
        {
            startPos = new Vector3(1 * spacing, 1, 1 * spacing);
            Instantiate(playerPrefab, startPos, Quaternion.identity);
        }

        GoalWorldPosition = new Vector3((width - 2) * spacing, 0.5f, (height - 2) * spacing);

        Instantiate(goalFlagPrefab, GoalWorldPosition, Quaternion.identity, mazeParent);
    }
    
}
