using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] private Image healthFill;
    [SerializeField] private Gradient healthGradient;

    public void UpdateHealth(int current, int max)
    {
        float fillAmount = (float)current / max;
        healthFill.fillAmount = fillAmount;
        healthFill.color = healthGradient.Evaluate(fillAmount);
    }
}