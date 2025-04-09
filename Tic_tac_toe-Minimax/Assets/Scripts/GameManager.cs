using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public GameObject winLinePrefab;
    private GameObject currentWinLine;
    private List<Vector2Int> winPositions = new List<Vector2Int>();
    public TextMeshProUGUI gameStatusText;
    public Button[,] gridButtons = new Button[3, 3];
    private bool isPlayerXTurn;
    private bool gameActive = true;
    private string playerSign;
    private string npcSign;
    private string difficulty;

    public Button resetButton;
    public Button mainMenuButton;

    void Start()
    {
        difficulty = PlayerPrefs.GetString("Difficulty", "Easy");
        playerSign = PlayerPrefs.GetString("PlayerSign", "X");
        npcSign = (playerSign == "X") ? "O" : "X";
        isPlayerXTurn = (playerSign == "X");
        gameStatusText.text = isPlayerXTurn ? $"Player {playerSign}'s Turn" : $"Player {npcSign}'s Turn";
        InitializeGrid();

        resetButton.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(false);

        resetButton.onClick.AddListener(ResetGame);
        mainMenuButton.onClick.AddListener(LoadMainMenu);

        if (!isPlayerXTurn)
        {
            StartCoroutine(AITurnDelayed());
        }
    }

    void InitializeGrid()
    {
        for (int i = 0; i < 3; i++) 
        {
            for (int j = 0; j < 3; j++) 
            {
                gridButtons[i, j] = GameObject.Find("Tile_" + i + "_" + j).GetComponent<Button>();
                int x = i;
                int y = j;
                gridButtons[i, j].onClick.AddListener(() => OnTileClicked(x, y));
            }
        }
    }

    void OnTileClicked(int x, int y)
    {
        // Povodna logika
        if (!gameActive || gridButtons[x, y].GetComponentInChildren<TextMeshProUGUI>().text != "")
            return;

        SetAllButtonsInteractable(false);
        gridButtons[x, y].GetComponentInChildren<TextMeshProUGUI>().text = isPlayerXTurn ? playerSign : npcSign;

        if (CheckWin())
        {
            gameStatusText.text = (isPlayerXTurn ? $"Player {playerSign}" : $"Player {npcSign}") + " Wins!";
            DrawWinLine();
            EndGame();
            return;
        }

        if (IsBoardFull())
        {
            gameStatusText.text = "Draw!";
            EndGame();
            return;
        }

        isPlayerXTurn = !isPlayerXTurn;
        gameStatusText.text = isPlayerXTurn ? $"Na ťahu je hráč {playerSign}" : $"Na ťahu je NPC {npcSign}";

        if (!isPlayerXTurn && gameActive)
        {
            StartCoroutine(AITurnDelayed());
        }
        else
        {
            SetAllButtonsInteractable(true);
        }
    }

    void EndGame()
    {
        gameActive = false;
        SetAllButtonsInteractable(false);
        resetButton.gameObject.SetActive(true);
        mainMenuButton.gameObject.SetActive(true);
    }

    void ResetGame()
    {
        if (currentWinLine != null) Destroy(currentWinLine);
        winPositions.Clear();

        // Reset vsetkych fields
        foreach (Button btn in gridButtons)
        {
            btn.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }

        // Reset premennych
        gameActive = true;
        isPlayerXTurn = (playerSign == "X");
        gameStatusText.text = isPlayerXTurn ? $"Player {playerSign}'s Turn" : $"Player {npcSign}'s Turn";
        resetButton.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(false);
        SetAllButtonsInteractable(true);

        if (!isPlayerXTurn)
        {
            StartCoroutine(AITurnDelayed());
        }
    }

    void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    private void SetAllButtonsInteractable(bool interactable)
    {
        foreach (Button btn in gridButtons)
        {
            btn.interactable = interactable;
        }
    }

    bool CheckWin()
{
    winPositions.Clear();

    // Kontrola riadkov
    for (int row = 0; row < 3; row++)
    {
        if (CompareCells(row, 0, row, 1, row, 2))
        {
            winPositions.Add(new Vector2Int(row, 0));
            winPositions.Add(new Vector2Int(row, 1));
            winPositions.Add(new Vector2Int(row, 2));
            return true;
        }
    }

    // Kontrola stlpcov
    for (int col = 0; col < 3; col++)
    {
        if (CompareCells(0, col, 1, col, 2, col))
        {
            winPositions.Add(new Vector2Int(0, col));
            winPositions.Add(new Vector2Int(1, col));
            winPositions.Add(new Vector2Int(2, col));
            return true;
        }
    }

    // Hlavna diagonala (zlava hore doprava dole)
    if (CompareCells(0, 0, 1, 1, 2, 2))
    {
        winPositions.Add(new Vector2Int(0, 0));
        winPositions.Add(new Vector2Int(1, 1));
        winPositions.Add(new Vector2Int(2, 2));
        return true;
    }

    // Vedlajsia diagonala (zprava hore dolava dole)
    if (CompareCells(0, 2, 1, 1, 2, 0))
    {
        winPositions.Add(new Vector2Int(0, 2));
        winPositions.Add(new Vector2Int(1, 1));
        winPositions.Add(new Vector2Int(2, 0));
        return true;
    }

    return false;
}

  void DrawWinLine()
{
    if (currentWinLine != null) Destroy(currentWinLine);

    // Najdite WinLineCanvas
    Canvas winCanvas = GameObject.Find("WinLineCanvas").GetComponent<Canvas>();
    
    // ciara v spravnom Canvas-e
    currentWinLine = Instantiate(winLinePrefab, winCanvas.transform);
    
    RectTransform lineRect = currentWinLine.GetComponent<RectTransform>();
    Image lineImage = currentWinLine.GetComponent<Image>();

    // Ziskaj pozicie prveho a posledneho vyherného polička
    Vector3 startPos = gridButtons[winPositions[0].x, winPositions[0].y].transform.position;
    Vector3 endPos = gridButtons[winPositions[2].x, winPositions[2].y].transform.position;

    // pozicia a velkost ciary
    lineRect.position = (startPos + endPos) / 2f;
    lineRect.sizeDelta = new Vector2(Vector3.Distance(startPos, endPos), 4f);

    // Nastavte rotáciu
    float angle = Mathf.Atan2(endPos.y - startPos.y, endPos.x - startPos.x) * Mathf.Rad2Deg;
    lineRect.rotation = Quaternion.Euler(0, 0, angle);
    lineImage.enabled = true;
}

    bool CompareCells(int x1, int y1, int x2, int y2, int x3, int y3)
    {
        string a = gridButtons[x1, y1].GetComponentInChildren<TextMeshProUGUI>().text;
        string b = gridButtons[x2, y2].GetComponentInChildren<TextMeshProUGUI>().text;
        string c = gridButtons[x3, y3].GetComponentInChildren<TextMeshProUGUI>().text;
        
        return a != "" && a == b && b == c;
    }

    IEnumerator AITurnDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        AITurn();
    }

    void AITurn()
    {
        if (!gameActive) return;

        switch (difficulty)
        {
            case "Easy": RandomMove(); break;
            case "Medium": BestMoveWithDepth(1); break;
            case "Impossible": BestMove(); break;
        }
    }

    void RandomMove()
    {
        int x, y;
        System.Random rand = new System.Random();
        do
        {
            x = rand.Next(0, 3); 
            y = rand.Next(0, 3); 
        } while (gridButtons[x, y].GetComponentInChildren<TextMeshProUGUI>().text != "");
        
        OnTileClicked(x, y);
    }

    void BestMove()
{
    if (!gameActive) return;

    // Skontrolujeme, ci je to prvy tah NPC
    bool isFirstMove = true;
    for (int i = 0; i < 3; i++)
    {
        for (int j = 0; j < 3; j++)
        {
            if (gridButtons[i, j].GetComponentInChildren<TextMeshProUGUI>().text != "")
            {
                isFirstMove = false;
                break;
            }
        }
        if (!isFirstMove) break;
    }

    // Ak je to prvy tah NPC, vyberieme random roh
    if (isFirstMove)
    {
        Vector2Int[] corners = { new Vector2Int(0, 0), new Vector2Int(0, 2), new Vector2Int(2, 2), new Vector2Int(2, 0) };
        System.Random rand = new System.Random();
        Vector2Int selectedCorner = corners[rand.Next(0, corners.Length)];

        OnTileClicked(selectedCorner.x, selectedCorner.y);
        return;
    }

    // Ak to nie je prvy tah, pokracujeme v Minimax algoritme
    int bestScore = int.MinValue;
    int bestMoveX = -1;
    int bestMoveY = -1;

    for (int i = 0; i < 3; i++)
    {
        for (int j = 0; j < 3; j++)
        {
            if (gridButtons[i, j].GetComponentInChildren<TextMeshProUGUI>().text == "")
            {
                gridButtons[i, j].GetComponentInChildren<TextMeshProUGUI>().text = npcSign;
                int score = Minimax(gridButtons, 0, false, int.MinValue, int.MaxValue);
                gridButtons[i, j].GetComponentInChildren<TextMeshProUGUI>().text = "";

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMoveX = i;
                    bestMoveY = j;
                }
            }
        }
    }

    if (bestMoveX != -1 && bestMoveY != -1)
    {
        OnTileClicked(bestMoveX, bestMoveY);
    }
}


   void BestMoveWithDepth(int depth)
{
    if (!gameActive) return;

    //  Skontrolujeme, ci ide o prvy tah NPC
    int filledTiles = 0;
    Vector2Int firstPlayerMove = new Vector2Int(-1, -1); // Pozícia prvého ťahu hráča
    List<Vector2Int> availableMoves = new List<Vector2Int>();

    for (int i = 0; i < 3; i++)
    {
        for (int j = 0; j < 3; j++)
        {
            string tileText = gridButtons[i, j].GetComponentInChildren<TextMeshProUGUI>().text;
            
            if (tileText == "") availableMoves.Add(new Vector2Int(i, j));
            else
            {
                filledTiles++;
                if (tileText == playerSign && filledTiles == 1) 
                {
                    firstPlayerMove = new Vector2Int(i, j); // Uložíme prvý ťah hráča
                }
            }
        }
    }

    // Ak NPC robí svoj úplne prvý ťah, aplikujeme pravidlá
    if (filledTiles == 1)
    {
        System.Random rand = new System.Random();
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        //  Ak hráč začal v strede (1,1), NPC vyberie náhodný zo strán
        if (firstPlayerMove == new Vector2Int(1, 1))
        {
            possibleMoves = new List<Vector2Int>
            {
                new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, 2), new Vector2Int(2, 1)
            };
        }
        //  Ak hráč začal v rohu, NPC vyberie z ostatných políčok okrem stredu
        else if (firstPlayerMove == new Vector2Int(0, 0) || firstPlayerMove == new Vector2Int(0, 2) ||
                 firstPlayerMove == new Vector2Int(2, 0) || firstPlayerMove == new Vector2Int(2, 2))
        {
            possibleMoves = availableMoves.Where(m => m != new Vector2Int(1, 1)).ToList();
        }
        //  Ak hráč začal v hornom strede (0,1), NPC vyberie: 1,0; 1,2; 2,0; 2,2
        else if (firstPlayerMove == new Vector2Int(0, 1))
        {
            possibleMoves = new List<Vector2Int>
            {
                new Vector2Int(1, 0), new Vector2Int(1, 2), new Vector2Int(2, 0), new Vector2Int(2, 2)
            };
        }
        // Ak hráč začal v ľavom strede (1,0), NPC vyberie: 0,1; 0,2; 2,1; 2,2
        else if (firstPlayerMove == new Vector2Int(1, 0))
        {
            possibleMoves = new List<Vector2Int>
            {
                new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(2, 1), new Vector2Int(2, 2)
            };
        }
        // Ak hráč začal v pravom strede (1,2), NPC vyberie: 0,1; 0,0; 2,0; 2,1
        else if (firstPlayerMove == new Vector2Int(1, 2))
        {
            possibleMoves = new List<Vector2Int>
            {
                new Vector2Int(0, 1), new Vector2Int(0, 0), new Vector2Int(2, 0), new Vector2Int(2, 1)
            };
        }
        // Ak hráč začal v dolnom strede (2,1), NPC vyberie: 1,0; 0,0; 0,2; 1,2
        else if (firstPlayerMove == new Vector2Int(2, 1))
        {
            possibleMoves = new List<Vector2Int>
            {
                new Vector2Int(1, 0), new Vector2Int(0, 0), new Vector2Int(0, 2), new Vector2Int(1, 2)
            };
        }

        // Ak je možný ťah, NPC ho vyberie
        if (possibleMoves.Count > 0)
        {
            Vector2Int chosenMove = possibleMoves[rand.Next(possibleMoves.Count)];
            OnTileClicked(chosenMove.x, chosenMove.y);
            return;
        }
    }

    // Ak to už nie je prvý ťah, NPC použije Minimax
    int bestScore = int.MinValue;
    List<Vector2Int> bestMoves = new List<Vector2Int>();

    for (int i = 0; i < 3; i++)
    {
        for (int j = 0; j < 3; j++)
        {
            if (gridButtons[i, j].GetComponentInChildren<TextMeshProUGUI>().text == "")
            {
                gridButtons[i, j].GetComponentInChildren<TextMeshProUGUI>().text = npcSign;
                int score = Minimax(gridButtons, depth, false, int.MinValue, int.MaxValue);
                gridButtons[i, j].GetComponentInChildren<TextMeshProUGUI>().text = "";

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMoves.Clear();
                    bestMoves.Add(new Vector2Int(i, j));
                }
                else if (score == bestScore)
                {
                    bestMoves.Add(new Vector2Int(i, j));
                }
            }
        }
    }

    // NPC vyberie najlepsi tah podla Minimaxu
    if (bestMoves.Count > 0)
    {
        System.Random rand = new System.Random();
        Vector2Int chosenMove = bestMoves[rand.Next(bestMoves.Count)];
        OnTileClicked(chosenMove.x, chosenMove.y);
    }
}


    int Minimax(Button[,] board, int depth, bool isMaximizing, int alpha, int beta)
    {
        if (CheckWin())
            return isMaximizing ? -1 : 1;

        if (IsBoardFull())
            return 0;

        if (difficulty == "Medium" && depth >= 3)
            return 0;

        int bestScore = isMaximizing ? int.MinValue : int.MaxValue;

        for (int i = 0; i < 3; i++) 
        {
            for (int j = 0; j < 3; j++) 
            {
                if (board[i, j].GetComponentInChildren<TextMeshProUGUI>().text == "")
                {
                    board[i, j].GetComponentInChildren<TextMeshProUGUI>().text = isMaximizing ? npcSign : playerSign;
                    int score = Minimax(board, depth + 1, !isMaximizing, alpha, beta);
                    board[i, j].GetComponentInChildren<TextMeshProUGUI>().text = "";

                    if (isMaximizing)
                    {
                        bestScore = Mathf.Max(bestScore, score);
                        alpha = Mathf.Max(alpha, bestScore);
                    }
                    else
                    {
                        bestScore = Mathf.Min(bestScore, score);
                        beta = Mathf.Min(beta, bestScore);
                    }

                    if (beta <= alpha)
                        break;
                }
            }
        }
        return bestScore;
    }

    bool IsBoardFull()
    {
        for (int i = 0; i < 3; i++) 
        {
            for (int j = 0; j < 3; j++) 
            {
                if (gridButtons[i, j].GetComponentInChildren<TextMeshProUGUI>().text == "")
                    return false;
            }
        }
        return true;
    }
}