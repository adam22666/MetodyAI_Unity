using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private float spawnRadius = 20f;
    [SerializeField] public LayerMask obstacleMask; 
    [SerializeField] public int maxEnemies = 10;

    private Transform playerTransform;
    private float timer;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            if (EnemyManager.Instance.activeEnemies.Count < EnemyManager.Instance.maxEnemies)
            {
                SpawnNPC();
                timer = 0f;
            }
        }
    }

  private void SpawnNPC()
{
    Vector3? safeSpawnPoint = FindSafeSpawnPosition();
    if (safeSpawnPoint.HasValue)
    {
        GameObject npc = Instantiate(
            EnemyManager.Instance.npcPrefab, 
            safeSpawnPoint.Value, 
            Quaternion.identity
        );
        EnemyManager.Instance.RegisterEnemy(npc.GetComponent<NPCController>());
    }
}

private Vector3? FindSafeSpawnPosition(int maxAttempts = 10)
{
    for (int i = 0; i < maxAttempts; i++)
    {
        Vector3 randomPoint = playerTransform.position + Random.insideUnitSphere * spawnRadius;
        if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out UnityEngine.AI.NavMeshHit hit, 2f, UnityEngine.AI.NavMesh.AllAreas))
        {
            if (!Physics.CheckSphere(hit.position, 1f, obstacleMask))
            {
                return hit.position;
            }
        }
    }
    return null;
}
}
