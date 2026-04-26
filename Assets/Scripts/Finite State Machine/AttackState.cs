
using UnityEngine;

public class AttackState : State
{

    private FSMAgent _agent;
    
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
