using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCController : MonoBehaviour
{
    private Vector2Int start;
    private Vector2Int goal;
    private int[,] maze;

    public float moveSpeed = 1.5f;
    public float rotationSpeed = 3.2f; // Rýchlosť rotácie NPC

    public void Initialize(Vector2Int start, Vector2Int goal, int[,] maze)
    {
        this.start = start;
        this.goal = goal;
        this.maze = maze;
        
        Debug.Log($"NPC initialized at {start}, moving towards {goal}");
        StartCoroutine(FindPath());
    }

    IEnumerator FindPath()
    {
        List<Vector2Int> path = AStarSearch(start, goal);
        
        if (path.Count == 0)
        {
            Debug.LogWarning("No path found to goal!");
            yield break;
        }

        foreach (Vector2Int step in path)
        {
            Vector3 targetPosition = new Vector3(step.x, 0.3f, step.y);
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                // Vypočítame smer od aktuálnej pozície k cieľovej pozícii
                Vector3 direction = (targetPosition - transform.position).normalized;
                // Vytvoríme cieľovú rotáciu tak, aby NPC hľadelo do smeru pohybu
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                // Zabezpečíme, že NPC zostáva vertikálne orientované
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

                // Pohyb NPC smerom k cieľovej pozícii
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed);
                yield return null;
            }
        }

        // Overenie, či NPC dosiahlo cieľ
        if (Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(goal.x, 0f, goal.y)) < 0.5f 
            && Mathf.Abs(transform.position.y - 0.3f) < 0.5f)
        {
            FindObjectOfType<GameManager>().OnNPCReachedExit();
        }

        Debug.Log("NPC has reached the goal!");
    }

    List<Vector2Int> AStarSearch(Vector2Int start, Vector2Int goal)
    {
        Debug.Log($"Starting A* search from {start} to {goal}");
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        PriorityQueue<Vector2Int> openSet = new PriorityQueue<Vector2Int>();
        openSet.Enqueue(start, 0);

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> costSoFar = new Dictionary<Vector2Int, int>();
        costSoFar[start] = 0;

        while (openSet.Count > 0)
        {
            Vector2Int current = openSet.Dequeue();

            if (current == goal)
            {
                Debug.Log("Goal reached during A* search!");
                break;
            }

            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (maze[neighbor.x, neighbor.y] == 1 || closedSet.Contains(neighbor))
                    continue;

                int newCost = costSoFar[current] + 1;
                if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                {
                    costSoFar[neighbor] = newCost;
                    int priority = newCost + Heuristic(neighbor, goal);
                    openSet.Enqueue(neighbor, priority);
                    cameFrom[neighbor] = current;
                }
            }
            closedSet.Add(current);
        }

        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int pathStep = goal;
        while (pathStep != start)
        {
            path.Insert(0, pathStep);
            if (!cameFrom.TryGetValue(pathStep, out pathStep))
            {
                Debug.LogWarning("Path reconstruction failed!");
                break;
            }
        }

        Debug.Log($"Path found: {string.Join(" -> ", path)}");
        return path;
    }

    IEnumerable<Vector2Int> GetNeighbors(Vector2Int current)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighbor = current + dir;
            if (neighbor.x >= 0 && neighbor.y >= 0 && neighbor.x < maze.GetLength(0) && neighbor.y < maze.GetLength(1))
                yield return neighbor;
        }
    }

    int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}
