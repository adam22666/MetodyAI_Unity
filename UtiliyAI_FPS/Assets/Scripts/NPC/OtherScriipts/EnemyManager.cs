using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    
   [Header("Spawn Settings")]
    [SerializeField] public GameObject npcPrefab;
    [SerializeField] public int maxEnemies = 10; 
    public HashSet<NPCController> activeEnemies = new HashSet<NPCController>();

    public int ActiveEnemyCount => activeEnemies.Count;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void RegisterEnemy(NPCController enemy)
    {
        activeEnemies.Add(enemy);
    }

    public void UnregisterEnemy(NPCController enemy)
    {
        activeEnemies.Remove(enemy);
    }

    public GameObject GetNpcPrefab() => npcPrefab;
}