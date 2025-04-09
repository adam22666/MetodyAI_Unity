using UnityEngine;
using UnityEngine.UI;    
using UnityEngine.SceneManagement; 

public class EndPoint : MonoBehaviour
{
    [Header("UI References")]
    public GameObject winPanel;

   private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        Debug.Log("Vyhral si! si v EndPoint.");

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("EndPoint: winPanel NIE JE priradeny!");
        }

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}


    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
