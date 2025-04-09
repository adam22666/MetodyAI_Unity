using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxLives = 3;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider bodyCollider;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Vector3 healthBarOffset = new Vector3(0, 2f, 0);

    private int currentLives;
    private BehaviorTree.Tree behaviorTree;
    private EnemyHealthUI healthUI;

    private void Awake()
    {
        currentLives = maxLives;
        behaviorTree = GetComponent<BehaviorTree.Tree>();
        
        // Inicializacia health baru
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxLives;
            healthSlider.value = maxLives;
            healthSlider.transform.position = transform.position + healthBarOffset;
            
            // Pridaj billboard efekt
            healthSlider.gameObject.AddComponent<Billboard>();
        }

        healthUI = GetComponentInChildren<EnemyHealthUI>();
    }

    public bool TakeHit(int damage = 1)
    {
        currentLives -= damage;
        Debug.Log($"NPC zasiahnuté! Zostáva: {currentLives}/{maxLives} HP");

        // anim zasah
        animator.SetTrigger("Hit1");
        
        UpdateHealthUI();

        if (currentLives <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentLives;
            healthSlider.transform.position = transform.position + healthBarOffset;
        }
        
        healthUI?.UpdateHealth(currentLives, maxLives);
    }

    private void Die()
    {
        Debug.Log("NPC zomrelo!");
        
        // anim. smrti
        animator.SetTrigger("Fall1");
        
        // Vypnutie kolizie a AI
        bodyCollider.enabled = false;
        behaviorTree.enabled = false;

        // Skrytie health baru
        if (healthSlider != null)
            Destroy(healthSlider.gameObject);

        Destroy(gameObject, 1.5f);
    }

    void LateUpdate()
    {
        if (healthSlider != null)
            healthSlider.transform.position = transform.position + healthBarOffset;
    }
}