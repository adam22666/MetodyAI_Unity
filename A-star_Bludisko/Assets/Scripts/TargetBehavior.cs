using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBehavior : MonoBehaviour 
{
    public int scoreAmount = 0; // skore za zničenie NPC
    public float timeAmount = 0.0f; // casovy bonus
    public GameObject explosionPrefab;
    
    void OnCollisionEnter(Collision newCollision)
{
    if (newCollision.gameObject.tag == "Projectile")
    {
        if (explosionPrefab)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }
        Destroy(newCollision.gameObject); 
        Destroy(gameObject); 

        FindObjectOfType<GameManager>().OnNPCDefeated();
    }
}

}
