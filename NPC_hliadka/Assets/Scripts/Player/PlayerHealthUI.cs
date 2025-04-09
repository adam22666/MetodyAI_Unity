using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public PlayerManager player;   
    public Slider healthSlider;    
    private int maxHP;

    void Start()
    {
        if (player == null)
            Debug.LogError("Player reference not set in PlayerHealthUI!");

        if (healthSlider == null)
            Debug.LogError("Health Slider reference not set in PlayerHealthUI!");

        maxHP = player.GetHealth(); 
        healthSlider.minValue = 0;
        healthSlider.maxValue = maxHP;
        healthSlider.value = maxHP;
    }

    void Update()
    {
        if (player != null && healthSlider != null)
        {
            healthSlider.value = player.GetHealth();
        }
    }
}
