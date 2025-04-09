using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour {
 public GameObject projectile; 	
 public float power = 10.0f; 	// sila/rýchlosť výstrelu
 public AudioClip shootSFX; 	
	
 void Update () {
  // ak bol stlačený kláves fire alebo medzera
  if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump")) {	
   // ak máme definovaný objekt projektil, čo snáď máme
   if (projectile) {
    // vytvoríme novú inštanciu (ako new) meter pred stredom kamery
    GameObject newProjectile = Instantiate(projectile, transform.position + 
   transform.forward, transform.rotation) as GameObject;
    // ak by projektil nemal RigidBody tak mu ho pridáme
    if (!newProjectile.GetComponent<Rigidbody>()) {
	newProjectile.AddComponent<Rigidbody>();
    }
    // nasmerujeme projektil podľa kamery s danou silou
    newProjectile.GetComponent<Rigidbody>().AddForce
(transform.forward * power, ForceMode.VelocityChange);
// ak je definovaný zvuk, tak ho prehrá
 if (shootSFX) {
   // ak má projektil komponent audiosource
   if (newProjectile.GetComponent<AudioSource> ()) {		
     // prehrá zvuk
     newProjectile.GetComponent<AudioSource> ().PlayOneShot (shootSFX);
   } else {
     // vytvorí sa nový zvuk a prehrá na pozícii projektilu 			
     // this automatically destroys itself once the audio is done
     AudioSource.PlayClipAtPoint (shootSFX, newProjectile.transform.position);
     }
    } 
   }
  }
 }
}
