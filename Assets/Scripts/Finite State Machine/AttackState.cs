using UnityEngine;

public class AttackState : State
{
    private FSMAgent _agent;

    float _timer = 0f;
    public AttackState(FSMAgent agent) : base(agent.FSM) 
    {
        _agent = agent;
    }

    public override void Enter()
    {
        Debug.Log("Iniciando secuencia de ataque");
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        if(_timer >= _agent._timeBetweenAttacks)
        {   
            float dist = Vector3.Distance(_agent.transform.position, _agent.TargetAgent.transform.position);
            if (dist <= _agent._meleeChaseAttackRadius)
            {
                _agent.TargetAgent.RecieveDamage(50f);
                _timer = 0f;
            }
            else if (dist <= _agent._rangeAttackRadius)
            {
                _agent.TargetAgent.RecieveDamage(10f);
                _timer = 0f;
            }
            else
            {
                _agent.FSM.ChangeState(_agent.Patrol);
                _timer = 0f;
            }
        }
    }

    public override void Exit() => Debug.Log("Saliendo de Attack");
}