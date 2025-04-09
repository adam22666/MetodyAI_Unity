using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject projectile; 
    public float power = 10.0f;  
    public AudioClip shootSFX;    

    void Update()
    {
        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump"))
        {
            if (projectile)
            {
                
                Transform cam = Camera.main.transform;

                //Vytvorenie projektilu meter pred kamerou
                GameObject newProjectile = Instantiate(
                    projectile,
                    cam.position + cam.forward * 0.5f, 
                    cam.rotation
                );

                // ak neni RigidBody, pridam
                if (!newProjectile.GetComponent<Rigidbody>())
                {
                    newProjectile.AddComponent<Rigidbody>();
                }

                
                //    aby „hlava“ nabaoja smerovala dopredu 
                newProjectile.transform.Rotate(0f, 180f, 0f);

                // Vystrelenie - v smere kamery
                newProjectile.GetComponent<Rigidbody>().AddForce(
                    cam.forward * power,
                    ForceMode.VelocityChange
                );

                if (shootSFX)
                {
                    
                    AudioSource projAudio = newProjectile.GetComponent<AudioSource>();
                    if (projAudio != null)
                    {
                        projAudio.PlayOneShot(shootSFX);
                    }
                    else
                    {
    
                        AudioSource.PlayClipAtPoint(shootSFX, newProjectile.transform.position);
                    }
                }
            }
        }
    }
}
