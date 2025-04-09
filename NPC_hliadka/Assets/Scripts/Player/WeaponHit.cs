using UnityEngine;
using System.Collections;

public class WeaponHit : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float cooldownTime = 1f;
    
    private Collider _weaponCollider;
    private bool _isOnCooldown = false;

    private void Start()
    {
        // Collider defaultne vypnuty, zapina sa s animaciou
        _weaponCollider = GetComponent<Collider>();
        if (_weaponCollider == null)
        {
            return;
        }

        // Vypneme ho, kaym sa animacia nerozhodne zapnut
        _weaponCollider.enabled = false;
    }

    public void EnableWeapon()
    {
        ToggleWeapon(true);
    }

    public void DisableWeapon()
    {
        ToggleWeapon(false);
    }

    private void ToggleWeapon(bool state)
    {
        // Ak je zbran v cooldowne, ignorujeme pokusy o zapnutie
        if (_isOnCooldown && state == true)
        {
            return;
        }

        if (_weaponCollider != null)
        {
            _weaponCollider.enabled = state;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isOnCooldown || !_weaponCollider.enabled) return;

        // Skontroluj, ci 'other' je v enemy vrstvach
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            var enemy = other.GetComponentInParent<EnemyManager>();
            if (enemy != null)
            {
                enemy.TakeHit(damage);

                StartCoroutine(WeaponCooldown());
            }
        }
    }

    private IEnumerator WeaponCooldown()
    {
        _isOnCooldown = true;

        // Vypneme collider (ak je zapnuty este)
        if (_weaponCollider.enabled)
        {
            _weaponCollider.enabled = false;
        }

        yield return new WaitForSeconds(cooldownTime);

        _isOnCooldown = false;
    }
}
