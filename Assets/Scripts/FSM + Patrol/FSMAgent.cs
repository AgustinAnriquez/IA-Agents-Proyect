using System;
using System.Collections.Generic;
using UnityEngine;

public class FSMAgent : Agent
{

    [SerializeField]
    private GameObject cheesePrefab;

    [SerializeField]
    private float _speed = 3f;
    public float Speed => _speed;
    [SerializeField]
    private float _timeBetweenAttacks = 3f;
    private float _rangeAttackRadius = 7f;
    private float _meleeChaseAttackRadius = 3f;
    [SerializeField]
    private PatrolData _patrolData;

    private readonly FiniteStateMachine _fsm = new();

    public FiniteStateMachine FSM => _fsm;

    public IdleState Idle { get; private set; }

    public PatrolState Patrol { get; private set; }

    public AttackState Attack { get; private set; }
    public PursuitState Pursuit { get; private set; }
    public CheeseState Cheese { get; private set; }

    private void Start() 
    {
        Idle = new(this);
        Patrol = new(_patrolData, this);
        Attack = new(this);
        Cheese = new CheeseState(cheesePrefab, this);
        _fsm.AddState(Idle);
        _fsm.AddState(Patrol);
        _fsm.AddState(Attack);
        _fsm.AddState(Cheese);
        _fsm.ChangeState(Idle);
    }

    
    private void Update()
    {
        _fsm.Update();
    }

    
}
