using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width = 21;
    public int height = 21;
    public GameObject wallPrefab;
    public GameObject pathPrefab;
    public GameObject startFlagPrefab;
    public GameObject goalFlagPrefab;

    public int[,] maze;
    public GameObject[,] tiles;
    /*
        void Start()
        {
            GenerateMaze();
            SpawnMaze();
            FindObjectOfType<MazeSolver>().Initialize(maze, tiles);
        }
     */   

    public void DoUIsTUFF()
    {
        GenerateMaze();
        SpawnMaze();
             FindObjectOfType<MazeSolver>().Initialize(maze, tiles);
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
                tiles[x, y] = Instantiate(prefab, pos, Quaternion.identity, transform);
            }
        }

        Vector3 startPos = new Vector3(1 * spacing, 0.5f, 1 * spacing);
        Vector3 goalPos = new Vector3((width - 2) * spacing, 0.5f, (height - 2) * spacing);

        Instantiate(startFlagPrefab, startPos, Quaternion.identity, transform);
        Instantiate(goalFlagPrefab, goalPos, Quaternion.identity, transform);
    }
}
