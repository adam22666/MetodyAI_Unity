using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    public int width = 20; 
    public int height = 20; 
    public GameObject wallPrefab; 
    public GameObject pathPrefab; 
    public GameObject npcPrefab; 
    public GameObject playerPrefab; 
    private int[,] maze; 
    private Vector2Int exitPosition; // vystup/ciel
    private GameObject npc; 
    private GameObject player;

    void Start()
    {
        GenerateMaze();
        AddExit();
        DrawMaze();
        SpawnNPC();
        SpawnPlayer();
    }

    public void GenerateMaze()
    {
        // Inicializacia mriezky
        maze = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = 1; // vsetko zacina jak stena
            }
        }

        // Randomized Depth-First Search
        int startX = Random.Range(0, width / 2) * 2; // Pociatocny bod na ceste
        int startY = Random.Range(0, height / 2) * 2;
        maze[startX, startY] = 0; // pociatocnas cesta
        GeneratePath(startX, startY);
    }

    void GeneratePath(int x, int y)
    {
        int[] directions = { 0, 1, 2, 3 }; // Hore, dole, vlavo, vpravo
        ShuffleArray(directions);

        foreach (int dir in directions)
        {
            int newX = x, newY = y;

            switch (dir)
            {
                case 0: newX = x; newY = y + 2; break; // Hore
                case 1: newX = x; newY = y - 2; break; // Dole
                case 2: newX = x - 2; newY = y; break; // Vlavo
                case 3: newX = x + 2; newY = y; break; // Vpravo
            }

            if (newX > 0 && newY > 0 && newX < width && newY < height && maze[newX, newY] == 1)
            {
                maze[newX, newY] = 0; // Nastavenie cesty
                maze[(x + newX) / 2, (y + newY) / 2] = 0; // Odstranenie steny medzi
                GeneratePath(newX, newY); // Rekurzivne volanie
            }
        }
    }

    void AddExit()
    {
        // Nahodne vyberieme stranu, kde bude vystup
        int side = Random.Range(0, 4); // 0 = hore, 1 = dole, 2 = vlavo, 3 = vpravo
        int exitX = 0, exitY = 0;
        Vector2Int insideDir = Vector2Int.zero;

        switch (side)
        {
            case 0: // horna strana
                exitX = Random.Range(1, width - 1);
                exitY = 0;
                insideDir = new Vector2Int(0, 1); 
                break;
            case 1: // dolna strana
                exitX = Random.Range(1, width - 1);
                exitY = height - 1;
                insideDir = new Vector2Int(0, -1);
                break;
            case 2: // lava strana
                exitX = 0;
                exitY = Random.Range(1, height - 1);
                insideDir = new Vector2Int(1, 0);
                break;
            case 3: // prava strana
                exitX = width - 1;
                exitY = Random.Range(1, height - 1);
                insideDir = new Vector2Int(-1, 0);
                break;
        }

        // Nastavenie bunky s vystupom ako cesty
        maze[exitX, exitY] = 0;
        exitPosition = new Vector2Int(exitX, exitY);

        // Otvorenie prvej bunky vo vnutri bludiska
        int adjacentX = exitX + insideDir.x;
        int adjacentY = exitY + insideDir.y;
        if (adjacentX >= 0 && adjacentX < width && adjacentY >= 0 && adjacentY < height)
        {
            maze[adjacentX, adjacentY] = 0;
            
            // zarucenie pristupu
            int nextX = adjacentX + insideDir.x;
            int nextY = adjacentY + insideDir.y;
            if (nextX >= 0 && nextX < width && nextY >= 0 && nextY < height && maze[nextX, nextY] == 1)
            {
                maze[nextX, nextY] = 0;
            }
        }

        // Vytvorenie objektu pre vystup
        GameObject exitObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        exitObject.transform.position = new Vector3(exitX, 0.02f, exitY);
        exitObject.transform.localScale = new Vector3(1, 0.2f, 1);
        exitObject.GetComponent<Renderer>().material.color = Color.green;
        exitObject.name = "Exit";

        Debug.Log($"Exit position set to: {exitPosition} on side {(side == 0 ? "Top" : side == 1 ? "Bottom" : side == 2 ? "Left" : "Right")}");
    }

    public void SpawnNPC()
    {
        Vector2Int npcPosition;
        int minDistanceFromExit = 20; // npc min. distance od ciela

        do
        {
            int npcX = Random.Range(1, width - 1);
            int npcY = Random.Range(1, height - 1);
            npcPosition = new Vector2Int(npcX, npcY);
        } 
        while (
            maze[npcPosition.x, npcPosition.y] != 0 || 
            Vector2Int.Distance(npcPosition, exitPosition) < minDistanceFromExit // NPC musi vyt dalej od ciela
        );

        Vector3 npcWorldPosition = new Vector3(npcPosition.x, 0.3f, npcPosition.y);
        npc = Instantiate(npcPrefab, npcWorldPosition, Quaternion.identity);
        npc.GetComponent<NPCController>().Initialize(npcPosition, exitPosition, maze);

        Debug.Log($"NPC spawned at {npcPosition}, goal set to {exitPosition}");
    }

    public void SpawnPlayer()
    {
        Vector2Int playerPosition;
        Vector2Int npcPosition = new Vector2Int(Mathf.RoundToInt(npc.transform.position.x), Mathf.RoundToInt(npc.transform.position.z));

        int minimumDistanceFromExit = 10; // Min. vzdialenost od exitu
        int maxAttempts = 1000; // max. pocet attempts na najdenie vhodnej pozicie
        int attempts = 0; // pocitadlo pokusov

        do
        {
            attempts++;

            int playerX = Random.Range(1, width - 1);
            int playerY = Random.Range(1, height - 1);
            playerPosition = new Vector2Int(playerX, playerY);

            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("Unable to find a valid player position. Using fallback position.");
                break;
            }

        } while (
            maze[playerPosition.x, playerPosition.y] != 0 || 
            Vector2Int.Distance(playerPosition, exitPosition) < minimumDistanceFromExit || 
            Vector2Int.Distance(playerPosition, npcPosition) < 5
        );

        Vector3 playerWorldPosition = new Vector3(playerPosition.x, 0.3f, playerPosition.y);
        player = Instantiate(playerPrefab, playerWorldPosition, Quaternion.identity);

        Debug.Log($"Player spawned at {playerPosition}, attempts: {attempts}, at least {minimumDistanceFromExit} units from exit at {exitPosition}");
    }

    void DrawMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x, 0, y);

                if (maze[x, y] == 1)
                {
                    Instantiate(wallPrefab, position, Quaternion.identity); // stena
                }
                else
                {
                    Instantiate(pathPrefab, position, Quaternion.identity); // cesta
                }
            }
        }
    }

    void ShuffleArray(int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int rand = Random.Range(0, array.Length);
            int temp = array[i];
            array[i] = array[rand];
            array[rand] = temp;
        }
    }
}
