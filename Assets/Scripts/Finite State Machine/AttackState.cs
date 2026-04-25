using System.Collections.Generic;
using UnityEngine;
using static Agent;

public class AttackState : State
{

    private FSMAgent _agent;
    private PatrolData _data;
    private int _currentIndex = 0;
    
    
    public AttackState(FSMAgent agent) : base(agent.FSM) 
    {
        _agent = agent;
    }
    public override void Enter()
    {
        Debug.LogError("Entre a Attack");
    }

    public override void Update()
    {
        
    }
    public override void Exit()
    {
        Debug.LogError("Sali de Attack");
    }
}
