using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;

public class TaskPatrol : Node
{
    private Transform _transform;
    private Animator _animator;
    private Transform[] _waypoints;
    private NavMeshAgent _agent;
    private int _currentWaypointIndex = 0;

    private float _waitTime = 1f;
    private float _waitCounter = 0f;
    private bool _waiting = false;

    public TaskPatrol(Transform transform, Transform[] waypoints)
    {
        _transform = transform;
        _animator = transform.GetComponent<Animator>();
        _waypoints = waypoints;
        _agent = transform.GetComponent<NavMeshAgent>();

        if (_agent == null)
        {
            Debug.LogError("NavMeshAgent not found on " + _transform.name);
            return;
        }

        // Nastav parametre agenta
        _agent.autoBraking = true;

        // start na prvom waypointe
        if (_waypoints.Length > 0)
            _agent.SetDestination(_waypoints[0].position);
    }

    public override NodeState Evaluate()
    {
        // Ak existuje 'target', znamena to, ze sme v chase, tak ho zrusme
        if (GetData("target") != null)
        {
            ClearData("target");
        }

        // animaccie pre patrol: Run = false, Attack = false
        if (_animator != null)
        {
            _animator.SetBool("Run", false);
            _animator.SetBool("Attack", false);
        }

        if (_agent == null || _waypoints.Length == 0)
        {
            state = NodeState.FAILURE;
            return state;
        }

        _agent.speed = GuardBT.speed; // bezna speed pri patrole

        if (_waiting)
        {
            _waitCounter += Time.deltaTime;
            if (_waitCounter >= _waitTime)
            {
                _waiting = false;
                if (_animator != null)
                {
                    _animator.SetBool("Idle", false);
                    _animator.SetBool("Walk", true);
                }
                _agent.isStopped = false;
            }
            state = NodeState.RUNNING;
            return state;
        }

        // Pokial necakame, tak anim = Walk
        if (_animator != null)
        {
            _animator.SetBool("Idle", false);
            _animator.SetBool("Walk", true);
        }

        // Kontrola ci sme dosli k waypointu
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            // cakanie
            _waitCounter = 0f;
            _waiting = true;

            // Idle
            if (_animator != null)
            {
                _animator.SetBool("Walk", false);
                _animator.SetBool("Idle", true);
            }

            _agent.isStopped = true;

            // next waypoinzt
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
            _agent.SetDestination(_waypoints[_currentWaypointIndex].position);
        }

        state = NodeState.RUNNING;
        return state;
    }
}
