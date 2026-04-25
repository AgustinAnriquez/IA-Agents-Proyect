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
        _agent._currentMode = SteeringModes.Pursuit;
        float dist = Vector3.Distance(_agent.transform.position, _agent.TargetAgent.transform.position);
        if (dist >= _agent.ViewRadius)
        {
            _agent.SetTargetAgent(null);
            _agent.FSM.ChangeState(_agent.Patrol);
        }
        else
        {
            _agent.FSM.ChangeState(_agent.Attack);
        }
    }
    public override void Exit()
    {
        Debug.LogError("Sali de Pursuit");
    }
}
