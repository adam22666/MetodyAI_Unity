using UnityEngine;
using TL.Core;

namespace TL.Core
{
    public class Context : MonoBehaviour
    {
        public static Context Instance { get; private set; }

        [Header("Player Settings")]
        [SerializeField] private string playerTag = "Player";
        public Transform Player { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            Initialize();
        }

        private void Initialize()
        {
            FindPlayer();
        }

        private void FindPlayer()
        {
            Player = GameObject.FindGameObjectWithTag(playerTag)?.transform;
    
    if (Player == null)
    {
        Debug.LogError("Hráč nebol nájdený! Skontrolujte tag.");
    }
    else
    {
        Debug.Log($"Hráč nájdený: {Player.name}"); 
    }
        }
        
    }
}