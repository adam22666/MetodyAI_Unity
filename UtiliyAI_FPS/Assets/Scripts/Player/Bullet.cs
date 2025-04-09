using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int damage = 50;
    [SerializeField] private float timeOut = 2f; 
    private bool hasDamaged = false;

    void Awake()
    {
        Invoke("DestroyNow", timeOut);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasDamaged) return;

        if (other.CompareTag("NPC"))
        {
            Health enemyHealth = other.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                hasDamaged = true;
                
                CancelInvoke("DestroyNow"); 
                Destroy(gameObject);
            }
        }
        else 
        {
        
        }
    }

    void DestroyNow()
    {
        if (!hasDamaged)
        {
            Destroy(gameObject);
        }
    }
}