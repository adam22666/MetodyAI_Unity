using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedObjectDestructor : MonoBehaviour
{
    public float timeOut = 1.0f; // o aky cas destruuje
    // jedna z metod, ked objekt vznikne podobne ako start
    void Awake()
    {
        // spusti DestroyNow po nastavenom case
        Invoke("DestroyNow", timeOut);
    }

    void DestroyNow()
    { // destroy the game Object
        Destroy(gameObject);
    }
}

