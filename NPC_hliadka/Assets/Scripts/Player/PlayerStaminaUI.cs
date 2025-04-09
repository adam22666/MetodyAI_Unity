using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStaminaUI : MonoBehaviour
{
    [Header("References")]
    public PlayerManager player;
    public Slider staminaSlider;
    public TMP_Text staminaText; 

    private void Start()
    {
        if (player == null)
            Debug.LogError("Player reference not set in PlayerStaminaUI!");

        if (staminaSlider == null)
            Debug.LogError("Stamina Slider reference not set in PlayerStaminaUI!");

        staminaSlider.minValue = 0;
        staminaSlider.maxValue = player.GetMaxStamina();
        staminaSlider.value = player.GetStamina();

        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (player == null || staminaSlider == null) return;

        staminaSlider.value = player.GetStamina();

        if (staminaText != null)
        {
            staminaText.text = $"{Mathf.RoundToInt(player.GetStamina())} / {Mathf.RoundToInt(player.GetMaxStamina())}";
        }

    }
}