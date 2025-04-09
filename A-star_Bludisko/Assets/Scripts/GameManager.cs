using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject endPanel;            
    public TextMeshProUGUI endMessage;   
    public Button resetButton;             

    private bool gameEnded = false;

    void Start()
    {
        if (endPanel != null)
        {
            endPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("EndPanel nie je priradený v inšpektore!");
        }

        if (resetButton == null)
        {
            Debug.LogWarning("ResetButton nie je priradený v inšpektore!");
        }
    }

    void EndGame(string message)
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (endPanel != null)
        {
            endPanel.SetActive(true);
        }
        if (endMessage != null)
        {
            endMessage.text = message;
        }
        Debug.Log("EndGame() zavolané s message: " + message);
    }

    public void OnNPCReachedExit()
    {
        if (!gameEnded)
        {
            gameEnded = true;
            EndGame("Prehral si!");
        }
    }

    public void OnNPCDefeated()
    {
        if (!gameEnded)
        {
            gameEnded = true;
            EndGame("Vyhral si!");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
