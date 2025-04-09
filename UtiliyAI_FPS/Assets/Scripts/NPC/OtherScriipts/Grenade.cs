using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Nastavenia výbuchu")]
    [SerializeField] private float fuseTime = 2f;       // cas do explozie
    [SerializeField] private float explosionRadius = 5f; 
    [SerializeField] private int damage = 25;

    [Header("Vizual/FX")]
    [SerializeField] private GameObject explosionEffect;

    private bool hasExploded = false;

    private void Start()
    {
        Invoke(nameof(Explode), fuseTime);
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        //efekt vybuchu
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider col in hits)
        {
            if (col.CompareTag("Player"))
            {
                PlayerHealth playerHealth = col.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
                
            }

            // rigid body + explozia
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(500f, transform.position, explosionRadius);
            }
        }

        Destroy(gameObject);
    }
}
