using System.Collections.Generic;
using UnityEngine;
using static Agent;

public class PursuitState : State
{

    private FSMAgent _agent;
    
    
    public PursuitState(FSMAgent agent) : base(agent.FSM) 
    {
        _agent = agent;
    }
    public override void Enter()
    {
        Debug.LogError("Entre a Pursuit");
    }

    public override void Update()
    {
        float dist = Vector3.Distance(_agent.transform.position, _agent.TargetAgent.transform.position);
        _agent._velocity += _agent.CalculatePursuit(_agent.TargetAgent);
        _agent.transform.position += _agent.Velocity * Time.deltaTime;
        _agent.transform.forward = _agent.Velocity;
        _agent.transform.position = Bounds.Instance.CalculateBoundPosition( _agent.transform.position);
        if (dist <= _agent.ViewRadius)
        {
            _agent.FSM.ChangeState(_agent.Attack);
        }
        else
        {
            _agent.FSM.ChangeState(_agent.Patrol);
        }
    }
    public override void Exit()
    {
        Debug.LogError("Sali de Pursuit");
    }
}
