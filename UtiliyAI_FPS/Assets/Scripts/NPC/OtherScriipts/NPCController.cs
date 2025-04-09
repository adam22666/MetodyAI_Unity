using UnityEngine;
using TL.UtilityAI;
using TL.Core;
using UnityEngine.AI;
using TL.UtilityAI.Actions;


[RequireComponent(typeof(NavMeshAgent), typeof(Health), typeof(AIBrain))]
public class NPCController : MonoBehaviour
{
    public enum State
    {
        Decide,
        Move,
        Execute,
        Dead
    }

    [Header("FSM Settings")]
    public State currentState = State.Decide;
    public float actionDistanceThreshold = 2f;

    [Header("Combat References")]
    public Transform weaponBarrel;
    public GameObject projectilePrefab;
    public GameObject grenadePrefab;

    [Header("Parameters")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] public bool forceShoot = false;
    [SerializeField] private float fireRate = 0.5f;
    private float lastShotTime;

    private float nextWanderTime = 0f;  
    private Vector3 currentRandomDest;
    private bool hasRandomDestination = false;
    public float lastGrenadeTimeGrenade = -Mathf.Infinity;

    public bool isReloading = false;

    public NavMeshAgent Agent { get; private set; }
    public Health Health { get;  private set; }
    public Stats Stats { get; private set; }
    public AIBrain AIBrain { get; private set; }

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Health = GetComponent<Health>();
        Stats = GetComponent<Stats>();
        AIBrain = GetComponent<AIBrain>();
        Health.OnDeath += HandleDeathEvent;
        Agent.updateRotation = false;
        Agent.stoppingDistance = 5f;

        InitializeStats();
    }
    private void HandleDeathEvent(GameObject npc)
    {
        if (npc == gameObject)
        {
            currentState = State.Dead;
            Agent.isStopped = true;
            EnemyManager.Instance.UnregisterEnemy(this);
            Destroy(gameObject);
        }
    }

    private void InitializeStats()
    {
       GetComponent<Stats>().InitializeRandomStats();
    }

    private void Update()
    {

        if (currentState == State.Dead) return;

        float distToPlayer = Vector3.Distance(transform.position, Context.Instance.Player.position);
        if (distToPlayer < 5f)
        {
            Agent.isStopped = true;
        }
        else
        {
            Agent.isStopped = false;
        }

        Agent.updateRotation = false;
        FaceTarget();
        FSMTick();
        if(forceShoot)
        {
            Shoot();
            forceShoot = false;
        }
    }

   private void FSMTick()
{
    switch (currentState)
    {
        case State.Decide:
            AIBrain.DecideBestAction();
            if (AIBrain.BestAction != null)
            {
                currentState = State.Move;
                Agent.isStopped = false;
                hasRandomDestination = false;
            }
            break;

        case State.Move:
            // paralelna cast akcie
            if (!AIBrain.finishedExecutingBestAction && AIBrain.BestAction != null)
            {
                AIBrain.BestAction.ParallelExecute(this);
            }

            // Generovanie random ciela iba ak aktualna akcia NIE JE CollectingAction
            if (!(AIBrain.BestAction is CollectingAction))
            {
                if (!hasRandomDestination || Time.time >= nextWanderTime || ReachedDestination(1f))
                {
                    currentRandomDest = GetRandomPositionAroundPlayer(5f, 30f);
                    Agent.SetDestination(currentRandomDest);
                    hasRandomDestination = true;
                    nextWanderTime = Time.time + 5f;
                }
            }
            
            // Prechod do Execute, ak nie je Reload alebo Collecting a agent dosiahol ciel
            if (!(AIBrain.BestAction is ReloadAction) && !(AIBrain.BestAction is CollectingAction) && Agent.remainingDistance <= Agent.stoppingDistance)
            {
                currentState = State.Execute;
            }
            // Ak je ReloadAction a finishedExecutingBestAction je nastavene, prejdeme do Decide
            if (AIBrain.BestAction is ReloadAction && AIBrain.finishedExecutingBestAction)
            {
                AIBrain.finishedExecutingBestAction = false;
                currentState = State.Decide;
            }
            break;

        case State.Execute:
            if (!AIBrain.finishedExecutingBestAction && AIBrain.BestAction != null)
            {
                AIBrain.BestAction.Execute(this);
            }
            else
            {
                AIBrain.finishedExecutingBestAction = false;
                currentState = State.Decide;
            }
            break;

        case State.Dead:
            break;
    }
}



private bool ReachedDestination(float threshold = 0.5f)
{
    // vracia true, ak je agent blízko destinácie
    if (Agent.pathPending) return false;
    return (Agent.remainingDistance <= Agent.stoppingDistance + threshold);
}

    private bool ShouldExecuteImmediately()
    {
        return Vector3.Distance(
            transform.position, 
            AIBrain.BestAction.RequiredDestination.position
        ) <= actionDistanceThreshold;
    }

    private bool ReachedDestination()
    {
        return !Agent.pathPending && 
               Agent.remainingDistance <= Agent.stoppingDistance;
    }

    private void FaceTarget()
{
    if (Context.Instance == null || Context.Instance.Player == null) return;

    // Vektor od NPC k hracovi (bez Y aby sa NPC nenaklanalo)
    Vector3 direction = (Context.Instance.Player.position - transform.position).normalized;
    direction.y = 0f;

    // Pozeraj na hráča
    Quaternion lookRotation = Quaternion.LookRotation(direction);

    // rotaciu s nejakou rychlostou
    transform.rotation = Quaternion.Slerp(
        transform.rotation,
        lookRotation,
        Time.deltaTime * rotationSpeed
    );
}
    public void Shoot()
{
    if (Stats.ammo <= 0) return;
    
    if (Time.time < lastShotTime + fireRate) return; 
    
    Instantiate(projectilePrefab, weaponBarrel.position, weaponBarrel.rotation);
    Stats.ammo--;
    lastShotTime = Time.time; 
}

    public void ThrowGrenade()
{
    if (Stats.grenades <= 0) return;
    if (grenadePrefab == null)
    {
        Debug.LogError("Granátový prefab nie je priradený!");
        return;
    }
    
    GameObject grenadeObj = Instantiate(grenadePrefab, weaponBarrel.position, Quaternion.identity);
    
    Rigidbody rb = grenadeObj.GetComponent<Rigidbody>();
    if (rb != null && Context.Instance != null && Context.Instance.Player != null)
    {
        // Smer od zbrane k playerovi
        Vector3 direction = (Context.Instance.Player.position - weaponBarrel.position).normalized;
        //  aby granat letel do vzduchu
        direction.y += 1f;
        direction = direction.normalized;
        
        float throwForce = 8f; // sila hodenia
        rb.AddForce(direction * throwForce, ForceMode.VelocityChange);
    }
    
    Stats.grenades--;
    Debug.Log($"Hod granátu! Zostáva granátov: {Stats.grenades}");
}

    public void Reload()
    {
        Stats.ammo = maxAmmo;
        Debug.Log("Prebitie!");
    }

    public void MoveToCover(Vector3 coverPosition)
    {
        Agent.SetDestination(coverPosition);
        currentState = State.Move;
    }

    private void HandleDeath()
    {
        Agent.isStopped = true;
    }
    private Vector3 GetRandomPositionAroundPlayer(float minDist, float maxDist)
    {
    if (Context.Instance == null || Context.Instance.Player == null)
        return transform.position; // fallback

    Vector3 playerPos = Context.Instance.Player.position;

    // random vzdialenosť r z intervalu [minDist, maxDist]
    float r = Random.Range(minDist, maxDist);

    //  náhodný uhol angle (0..2π)
    float angle = Random.Range(0f, Mathf.PI * 2);

    // 3) offset
    float offsetX = r * Mathf.Cos(angle);
    float offsetZ = r * Mathf.Sin(angle);

    Vector3 candidatePos = playerPos + new Vector3(offsetX, 0, offsetZ);

    return candidatePos;
    }
}