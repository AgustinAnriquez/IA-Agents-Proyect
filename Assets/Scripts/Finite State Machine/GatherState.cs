using UnityEngine;
using static Agent;

public class GatherState : State
{
    private FSMAgent _agent;
    private float _gatherDistance = 3f;
    
    float _timeToChangePatrol = 3f;

    float _timer = 0f;
    public GatherState(FSMAgent agent) : base(agent.FSM) 
    {
        _agent = agent;
    }

    public override void Enter()
    {
        Debug.LogError("Entre a Gather");
    }

    public override void Exit()
    {
        Debug.LogError("Sali de Gather");
    }

    public override void Update()
    {
        float dist = Vector3.Distance(_agent.transform.position, _agent.TargetAgent.transform.position);
        Debug.Log(dist);
        _agent._velocity += _agent.CalculateArrive(_agent.TargetAgent.transform.position);
        _agent.transform.position += _agent.Velocity * Time.deltaTime;
        _agent.transform.forward = _agent.Velocity;
        _agent. transform.position = Bounds.Instance.CalculateBoundPosition( _agent.transform.position);
        if (dist <= _gatherDistance)
        {
            _timer += Time.deltaTime;
            if(_timer >= _timeToChangePatrol)
            {   
                _agent.TargetAgent.Destroy();
                _agent.FSM.ChangeState(_agent.Patrol);
            }
        }
    }
}