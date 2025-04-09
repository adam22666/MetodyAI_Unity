using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;

public class TaskGoToTarget : Node
{
    private Transform _transform;
    private NavMeshAgent _agent;
    private Animator _animator;
    private GuardBT guardBT;

    private float defaultChaseMultiplier = 1.25f;
    // multiplikátor pre runnera
    private float runnerMultiplier = 1.75f;

    public TaskGoToTarget(Transform transform)
    {
        _transform = transform;
        _agent = _transform.GetComponent<NavMeshAgent>();
        _animator = _transform.GetComponent<Animator>();

        //
        guardBT = _transform.GetComponent<GuardBT>();

        if (_agent == null)
            Debug.LogError("No NavMeshAgent attached to " + _transform.name);

        if (guardBT == null)
            Debug.LogWarning("GuardBT not found on " + _transform.name + "! AttackRange won't be used.");
    }

    public override NodeState Evaluate()
    {
        // Zistime ci existuje target
        Transform target = (Transform)GetData("target");
        if (target == null)
        {
            // ziadny ciel -> FAILURE -> Patrol
            if (_animator != null)
            {
                _animator.SetBool("Run", false);
                _animator.SetBool("Walk", false);
            }

            state = NodeState.FAILURE;
            return state;
        }

        // nastavenie NPC 2. typ (Runner)
        bool isRunner = _transform.CompareTag("NPC");

        if (_animator != null)
        {
            _animator.SetBool("Idle", false);
            _animator.SetBool("Attack", false);

            if (isRunner)
            {
                _animator.SetBool("Run", true);
                _animator.SetBool("Walk", false);
            }
            else
            {
                _animator.SetBool("Run", false);
                _animator.SetBool("Walk", true);
            }
        }

        // NavMeshAgent pohyb
        if (_agent != null)
        {
            _agent.isStopped = false;

            // rachlost cahse
            if (isRunner)
                _agent.speed = GuardBT.speed * runnerMultiplier;
            else
                _agent.speed = GuardBT.speed * defaultChaseMultiplier;

            //GuardBT.attackRange
            if (guardBT != null)
                _agent.stoppingDistance = guardBT.attackRange;
            else
                _agent.stoppingDistance = 1f; // fallback

            _agent.SetDestination(target.position);

            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                state = NodeState.SUCCESS;
                return state;
            }
        }
        else
        {
            // 5) Fallback bez NavMesh
            float chaseSpeed = isRunner
                ? GuardBT.speed * runnerMultiplier
                : GuardBT.speed * defaultChaseMultiplier;

            float stopDist = (guardBT != null) ? guardBT.attackRange : 1f;

            float distance = Vector3.Distance(_transform.position, target.position);
            if (distance > stopDist)
            {
                // priblizime
                Vector3 dir = (target.position - _transform.position).normalized;
                _transform.position += dir * (chaseSpeed * Time.deltaTime);

                dir.y = 0;
                if (dir.sqrMagnitude > 0.01f)
                {
                    Quaternion lookRot = Quaternion.LookRotation(dir);
                    _transform.rotation = Quaternion.Slerp(_transform.rotation, lookRot, 5f * Time.deltaTime);
                }
            }
            else
            {
                // Sme v stopDist => SUCCESS
                state = NodeState.SUCCESS;
                return state;
            }
        }

        state = NodeState.RUNNING;
        return state;
    }
}
