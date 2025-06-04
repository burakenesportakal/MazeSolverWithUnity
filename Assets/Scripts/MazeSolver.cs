using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeSolver : MonoBehaviour
{
    private int[,] maze;
    private GameObject[,] tiles;

    private Vector2Int start;
    private Vector2Int goal;

    public float stepDelay = 0.01f;

    public void Initialize(int[,] mazeData, GameObject[,] tileRefs)
    {
        maze = mazeData;
        tiles = tileRefs;

        start = new Vector2Int(1, 1);
        goal = new Vector2Int(maze.GetLength(0) - 2, maze.GetLength(1) - 2);

        SolverType solver = GameManager.Instance.selectedSolver;

        if (solver == SolverType.Manual)
            return;

        switch (solver)
        {
            case SolverType.DFS: StartCoroutine(SolveDFS()); break;
            case SolverType.BFS: StartCoroutine(SolveBFS()); break;
            case SolverType.Dijkstra: StartCoroutine(SolveDijkstra()); break;
            case SolverType.AStar: StartCoroutine(SolveAStar()); break;
        }
    }

    IEnumerator SolveDFS()
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        bool[,] visited = new bool[maze.GetLength(0), maze.GetLength(1)];
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        stack.Push(start);
        Timer.Instance.StartTimer();

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop();
            if (!IsValid(current) || visited[current.x, current.y]) continue;

            visited[current.x, current.y] = true;
            tiles[current.x, current.y].GetComponent<Renderer>().material.color = Color.blue;
            yield return new WaitForSeconds(stepDelay);

            if (current == goal)
            {
                Timer.Instance.StopTimer();
                StartCoroutine(TracePath(current, cameFrom));
                yield break;
            }

            foreach (Vector2Int dir in GetDirections())
            {
                Vector2Int next = current + dir;
                if (IsValid(next) && !visited[next.x, next.y])
                {
                    stack.Push(next);
                    if (!cameFrom.ContainsKey(next)) cameFrom[next] = current;
                }
            }
        }
    }

    IEnumerator SolveBFS()
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        bool[,] visited = new bool[maze.GetLength(0), maze.GetLength(1)];
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        queue.Enqueue(start);
        visited[start.x, start.y] = true;
        Timer.Instance.StartTimer();

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            tiles[current.x, current.y].GetComponent<Renderer>().material.color = Color.blue;
            yield return new WaitForSeconds(stepDelay);

            if (current == goal)
            {
                Timer.Instance.StopTimer();
                StartCoroutine(TracePath(current, cameFrom));
                yield break;
            }

            foreach (Vector2Int dir in GetDirections())
            {
                Vector2Int next = current + dir;
                if (IsValid(next) && !visited[next.x, next.y])
                {
                    queue.Enqueue(next);
                    visited[next.x, next.y] = true;
                    cameFrom[next] = current;
                }
            }
        }
    }

    IEnumerator SolveDijkstra()
    {
        var pq = new PriorityQueue<Vector2Int>();
        var distance = new Dictionary<Vector2Int, int>();
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        bool[,] visited = new bool[maze.GetLength(0), maze.GetLength(1)];

        pq.Enqueue(start, 0);
        distance[start] = 0;
        Timer.Instance.StartTimer();

        while (pq.Count > 0)
        {
            Vector2Int current = pq.Dequeue();

            if (visited[current.x, current.y]) continue;
            visited[current.x, current.y] = true;

            tiles[current.x, current.y].GetComponent<Renderer>().material.color = Color.blue;
            yield return new WaitForSeconds(stepDelay);

            if (current == goal)
            {
                Timer.Instance.StopTimer();
                StartCoroutine(TracePath(current, cameFrom));
                yield break;
            }

            foreach (var dir in GetDirections())
            {
                Vector2Int next = current + dir;
                if (!IsValid(next) || visited[next.x, next.y]) continue;

                int newDist = distance[current] + 1;

                if (!distance.ContainsKey(next) || newDist < distance[next])
                {
                    distance[next] = newDist;
                    pq.Enqueue(next, newDist);
                    cameFrom[next] = current;
                }
            }
        }
    }

    IEnumerator SolveAStar()
    {
        var openSet = new PriorityQueue<Vector2Int>();
        var gScore = new Dictionary<Vector2Int, int>();
        var fScore = new Dictionary<Vector2Int, float>();
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        bool[,] visited = new bool[maze.GetLength(0), maze.GetLength(1)];

        gScore[start] = 0;
        fScore[start] = Heuristic(start, goal);
        openSet.Enqueue(start, fScore[start]);
        Timer.Instance.StartTimer();

        while (openSet.Count > 0)
        {
            Vector2Int current = openSet.Dequeue();

            if (visited[current.x, current.y]) continue;
            visited[current.x, current.y] = true;

            tiles[current.x, current.y].GetComponent<Renderer>().material.color = Color.blue;
            yield return new WaitForSeconds(stepDelay);

            if (current == goal)
            {
                Timer.Instance.StopTimer();
                StartCoroutine(TracePath(current, cameFrom));
                yield break;
            }

            foreach (Vector2Int dir in GetDirections())
            {
                Vector2Int next = current + dir;
                if (!IsValid(next) || visited[next.x, next.y]) continue;

                int tentativeG = gScore[current] + 1;
                if (!gScore.ContainsKey(next) || tentativeG < gScore[next])
                {
                    gScore[next] = tentativeG;
                    fScore[next] = tentativeG + Heuristic(next, goal);
                    openSet.Enqueue(next, fScore[next]);
                    cameFrom[next] = current;
                }
            }
        }
    }

    IEnumerator TracePath(Vector2Int current, Dictionary<Vector2Int, Vector2Int> cameFrom)
    {
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            tiles[current.x, current.y].GetComponent<Renderer>().material.color = Color.green;
            yield return new WaitForSeconds(stepDelay / 2f);
        }
    }

    bool IsValid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < maze.GetLength(0) &&
               pos.y >= 0 && pos.y < maze.GetLength(1) &&
               maze[pos.x, pos.y] == 0;
    }

    Vector2Int[] GetDirections()
    {
        return new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
    }

    float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // Manhattan
    }
}
