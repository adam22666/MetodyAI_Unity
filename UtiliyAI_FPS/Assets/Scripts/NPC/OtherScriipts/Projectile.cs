using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 50f;
    [SerializeField] private int damage = 5;
    [SerializeField] private float lifeTime = 2f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }

   private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        Debug.Log("Projectile: Zasiahol hráča!");
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }
    
    Destroy(gameObject);
}


}