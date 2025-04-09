using UnityEngine;
using TL.UI;

namespace TL.Core
{
    public class Stats : MonoBehaviour
    {
        [Header("Combat Stats")]
        [SerializeField] private int _health = 100;
        public int maxHealth = 100;
        [Header("Ammo Settings")]
        public int maxAmmo = 30; 
        public int maxGrenades = 3;

         public int health
        {
            get => _health;
            set
            {
                _health = Mathf.Clamp(value, 0, 100);
                OnStatValueChanged?.Invoke();
            }
        }

        [SerializeField] private int _ammo = 30;
        public int ammo
        {
            get => _ammo;
            set
            {
                _ammo = Mathf.Clamp(value, 0, maxAmmo); 
                OnStatValueChanged?.Invoke();
            }
        }

        [SerializeField] private int _grenades = 3;
        public int grenades
        {
            get => _grenades;
            set
            {
                _grenades = Mathf.Clamp(value, 0, maxGrenades); 
                OnStatValueChanged?.Invoke();
            }
        }

        [Header("UI Reference")]
        [SerializeField] private Billboard billboard;

        public delegate void StatValueChangedHandler();
        public event StatValueChangedHandler OnStatValueChanged;

        private void Start()
        {
            InitializeRandomStats();
        }

        public void InitializeRandomStats()
        {
            health = maxHealth;
            ammo = maxAmmo;
            grenades = maxGrenades;
        }

        private void OnEnable() => OnStatValueChanged += UpdateDisplayText;
        private void OnDisable() => OnStatValueChanged -= UpdateDisplayText;

        public void TakeDamage(int damage)
        {
            health -= damage;
            if(health <= 0) Die();
        }

        private void Die()
        {
            Destroy(gameObject);
        }

        private void UpdateDisplayText()
        {
            billboard.UpdateStatsText(health, ammo, grenades);
        }

        public bool IsCriticalHealth => health <= 20;
        public bool NeedsReload => ammo <= 15;
        public bool HasGrenades => grenades > 0;
    }
}