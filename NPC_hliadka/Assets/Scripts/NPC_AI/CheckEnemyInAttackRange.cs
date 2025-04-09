using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class CheckEnemyInAttackRange : Node
{
    private Transform _transform;
    private Animator _animator;
    private float _range;

    public CheckEnemyInAttackRange(Transform transform, float range)
    {
        _transform = transform;
        _animator = transform.GetComponent<Animator>();
        _range = range;
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");
        if (t == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        // target moze byt destroyed => MissingReference
        Transform target = (Transform)t;

        // Skontrolujeme, ci nebol zniceny
        if (target == null || target.gameObject == null)
        {
            // Zrusime ho  a vrátime FAIL
            ClearData("target");
            state = NodeState.FAILURE;
            return state;
        }

        float distance = Vector3.Distance(_transform.position, target.position);
        if (distance <= _range)
        {
            _animator.SetBool("Attack", true);
            _animator.SetBool("Walk", false);
            _animator.SetBool("Run", false);

            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}
