using UnityEngine;
using TL.Core; 

public class Health : MonoBehaviour
{
    public delegate void DeathHandler(GameObject npc);
    public event DeathHandler OnDeath;

    [SerializeField] public float maxHealth = 100f;
    public float currentHealth;
    public bool IsDead { get; private set; }

    private Stats stats;

    private void Start()
    {
        currentHealth = maxHealth;
        stats = GetComponent<Stats>();
        if(stats != null) stats.health = Mathf.RoundToInt(currentHealth);
    }

    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        stats.health = Mathf.RoundToInt((currentHealth / maxHealth) * 100);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        IsDead = true;
        OnDeath?.Invoke(gameObject);
    }
}