using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeSolver : MonoBehaviour
{
    private int[,] maze;
    private GameObject[,] tiles;
    private Stack<Vector2Int> stack;
    private bool[,] visited;
    private Dictionary<Vector2Int, Vector2Int> cameFrom;

    public float stepDelay = 0.05f;
    private Vector2Int start;
    private Vector2Int goal;

    public void Initialize(int[,] mazeData, GameObject[,] tileRefs)
    {
        maze = mazeData;
        tiles = tileRefs;
        stack = new Stack<Vector2Int>();
        visited = new bool[maze.GetLength(0), maze.GetLength(1)];
        cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        start = new Vector2Int(1, 1);
        goal = new Vector2Int(maze.GetLength(0) - 2, maze.GetLength(1) - 2);

        StartCoroutine(Solve());
    }

    IEnumerator Solve()
    {
        stack.Push(start);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop();

            if (!IsValid(current) || visited[current.x, current.y])
                continue;

            visited[current.x, current.y] = true;
            tiles[current.x, current.y].GetComponent<Renderer>().material.color = Color.blue;
            yield return new WaitForSeconds(stepDelay);

            if (current == goal)
            {
                StartCoroutine(TracePath(current));
                yield break;
            }

            foreach (var dir in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int next = current + dir;
                if (IsValid(next) && !visited[next.x, next.y])
                {
                    stack.Push(next);
                    if (!cameFrom.ContainsKey(next))
                        cameFrom[next] = current;
                }
            }
        }
    }

    IEnumerator TracePath(Vector2Int current)
    {
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            tiles[current.x, current.y].GetComponent<Renderer>().material.color = Color.green;
            yield return new WaitForSeconds(stepDelay / 2);
        }
    }

    bool IsValid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < maze.GetLength(0) &&
               pos.y >= 0 && pos.y < maze.GetLength(1) &&
               maze[pos.x, pos.y] == 0;
    }
}