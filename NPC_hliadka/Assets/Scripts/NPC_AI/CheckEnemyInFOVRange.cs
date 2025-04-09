using UnityEngine;
using BehaviorTree;

public class CheckEnemyInFOVRange : Node
{
    private static int _enemyLayerMask = 1 << 6;
    private const float MAX_CHASE_DISTANCE = 60f;
    private const float FIELD_OF_VIEW = 160f;
    private const float SIGHT_TIMEOUT = 2f;
    private const float DEFAULT_CLOSE_PROXIMITY = 30f;

    private Transform _transform;
    private Animator _animator;
    private float _lastSeenTime;

    public CheckEnemyInFOVRange(Transform transform)
    {
        _transform = transform;
        _animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        float effectiveCloseProximity = PlayerController.isWalking ? 15f : DEFAULT_CLOSE_PROXIMITY;

        object t = GetData("target");
     
        if (t != null)
        {
            Transform target = (Transform)t;
            Vector3 directionToTarget = (target.position - _transform.position).normalized;
            float angleToTarget = Vector3.Angle(_transform.forward, directionToTarget);
            float distance = Vector3.Distance(_transform.position, target.position);

            bool isCloseProximity = distance <= effectiveCloseProximity;
            bool inFOV = angleToTarget <= FIELD_OF_VIEW / 2;
            bool inChaseRange = distance <= MAX_CHASE_DISTANCE;

            if ((inFOV && inChaseRange) || isCloseProximity)
            {
                _lastSeenTime = Time.time;
                state = NodeState.SUCCESS;
                return state;
            }

            if (Time.time - _lastSeenTime > SIGHT_TIMEOUT)
            {
                ClearData("target");
                ResetChaseState();
                state = NodeState.FAILURE;
                return state;
            }

            state = NodeState.SUCCESS;
            return state;
        }

        float range = Mathf.Max(GuardBT.fovRange, effectiveCloseProximity);
        Collider[] colliders = Physics.OverlapSphere(_transform.position, range, _enemyLayerMask);

        foreach (Collider collider in colliders)
        {
            Vector3 targetPos = collider.transform.position;
            float distance = Vector3.Distance(_transform.position, targetPos);
            Vector3 directionToTarget = (targetPos - _transform.position).normalized;
            float angleToTarget = Vector3.Angle(_transform.forward, directionToTarget);

            if (angleToTarget <= FIELD_OF_VIEW / 2 || distance <= effectiveCloseProximity)
            {
                parent.parent.SetData("target", collider.transform);
                _lastSeenTime = Time.time;

                _animator.SetBool("Walk", true);
                _animator.SetBool("Attack", false);

                state = NodeState.SUCCESS;
                return state;
            }
        }

        state = NodeState.FAILURE;
        return state;
    }

    private void ResetChaseState()
    {
        // Zastavime NavMeshAgent
        UnityEngine.AI.NavMeshAgent agent = _transform.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }
        _animator.SetBool("Walk", false);
        _animator.SetBool("Attack", false);
    }

    //NPC kruh, ciel vzdialenost v scene view...
    void OnDrawGizmosSelected()
    {
        if (_transform == null) return;
        
        float effectiveCloseProximity = PlayerController.isWalking ? 15f : DEFAULT_CLOSE_PROXIMITY;

        Gizmos.color = Color.yellow;
        Vector3 forward = _transform.forward * GuardBT.fovRange;
        Quaternion leftRot = Quaternion.AngleAxis(-FIELD_OF_VIEW/2, Vector3.up);
        Quaternion rightRot = Quaternion.AngleAxis(FIELD_OF_VIEW/2, Vector3.up);
        Gizmos.DrawLine(_transform.position, _transform.position + leftRot * forward);
        Gizmos.DrawLine(_transform.position, _transform.position + rightRot * forward);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_transform.position, effectiveCloseProximity);
    }
}
