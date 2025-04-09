using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;

public class TaskAttack : Node
{
    private Animator _animator;
    private NavMeshAgent _agent;
    
    private float _attackInterval = 1f;
    private float _attackTimer = 0f;
    private float offsetAngle = -13f;

    public TaskAttack(Transform transform)
    {
        _animator = transform.GetComponent<Animator>();
        _agent = transform.GetComponent<NavMeshAgent>();
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        if (target == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        // Zastavenie pohybu
        if (_agent != null) 
            _agent.isStopped = true;

        // Rotacia s offsetom
        Vector3 direction = (target.position - _animator.transform.position).normalized;
        direction.y = 0f;
        
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, offsetAngle, 0);
            _animator.transform.rotation = Quaternion.Slerp(
                _animator.transform.rotation,
                lookRotation,
                Time.deltaTime * 5f
            );
        }

        // Animacia attack
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= _attackInterval)
        {
            _animator.SetTrigger("Attack");
            _attackTimer = 0f;
        }

        state = NodeState.RUNNING;
        return state;
    }
}