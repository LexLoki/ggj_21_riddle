using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CustomerBehaviour : MonoBehaviour
{

    public float Speed;
    public int ID;
    public Charade Riddle;
    public float MaxWaitTime = 3f;

    public bool IsWaiting { get { return _waitTimer >= 0f; } }

    public SpriteRenderer ExcMark;

    private Animator _animator;

    private Transform _target;


    private Action<CustomerBehaviour> _onReach;
    private Action<CustomerBehaviour> _onOverwait;

    private string _targetState = null;
    private float _waitTimer = -1f;
    

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetMovement(Transform target, Action<CustomerBehaviour> onReachCallback)
    {
        _target = target;
        CancelWait();
        _targetState = target.position.x > transform.position.x ? "movesRight" : "movesLeft";
        _onReach = onReachCallback;
    }

    private void SetIdle()
    {
        _target = null;
        _targetState = "setIdle";
    }

    public void StartWait(Action<CustomerBehaviour> onOverWaitCallback)
    {
        _waitTimer = 0f;
        _onOverwait = onOverWaitCallback;
    }

    public void CancelWait()
    {

    }

    private void Update()
    {
        if (_target)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target.position, Time.deltaTime * Speed);
            if (Vector3.Distance(transform.position, _target.position) < .001f)
            {
                SetIdle();
                _onReach?.Invoke(this);
            }
        }
        else if (_waitTimer >= 0f)
        {
            _waitTimer += Time.deltaTime;
            if(_waitTimer > MaxWaitTime)
            {
                CancelWait();
                _onOverwait?.Invoke(this);
            }
        }
    }

    private void LateUpdate()
    {
        if (_targetState != null)
        {
            _animator.SetTrigger(_targetState);
            _targetState = null;
        }
    }
}
