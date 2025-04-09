using System.Collections;
using UnityEngine;

public class SwordHit : MonoBehaviour
{
    public int damage = 10;
    private Collider _collider;

    void Start()
    {
        _collider = GetComponent<Collider>();
        _collider.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _collider.enabled)
        {
            _collider.enabled = false;
            other.GetComponent<PlayerManager>()?.TakeHit(damage);
            StartCoroutine(ResetCollider());
        }
    }

    private IEnumerator ResetCollider()
    {
        yield return new WaitForSeconds(0.4f);
        _collider.enabled = true;
    }
}
