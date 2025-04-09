using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 30;
    private int currentHealth;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaDrainRun = 10f;
    public float staminaRegenWalk = 5f;
    public float staminaRegenIdle = 10f;
  
    private float currentStamina;

    [Header("Game Over UI")]
    public GameObject gameOverPanel; 

    private void Awake()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    public bool TakeHit(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Hráč zasiahnutý! Zostávajúce HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    private void Die()
    {
        Debug.Log("Hráč zomrel!");
        ShowGameOverScreen();
    }

    private void ShowGameOverScreen()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("GameOverPanel nie je nastaveny v PlayerManager!");
        }

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public int GetHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public float GetStamina() => currentStamina;
    public float GetMaxStamina() => maxStamina;

    public void UpdateStamina(int movementState)
    {
        switch (movementState)
        {
            case 2: // BEH
                currentStamina -= staminaDrainRun * Time.deltaTime;
                break;
            case 1: // CHÔDZA
                currentStamina += staminaRegenWalk * Time.deltaTime;
                break;
            default: // IDLE
                currentStamina += staminaRegenIdle * Time.deltaTime;
                break;
        }

        // Zaokrúhli na 2 desatinne miesta pre presnost
        currentStamina = Mathf.Round(currentStamina * 100f) / 100f;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }
}
