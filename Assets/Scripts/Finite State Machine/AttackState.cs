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
        Debug.Log("Iniciando secuencia de ataque");
    }

    public override void Update()
    {
        float distance = Vector3.Distance(_agent.transform.position, _agent.TargetAgent.transform.position);
        if (distance <= _agent._meleeChaseAttackRadius)
        {
            ExecuteMeleeAttack();
        }
        else if (distance <= _agent._rangeAttackRadius)
        {
            ExecuteRangeAttack();
        }
    }

    private void ExecuteMeleeAttack()
    {
        Debug.Log("¡Ataque Melee exitoso!");
        FinishAttack();
    }

    private void ExecuteRangeAttack()
    {
        Debug.Log("¡Disparo de Rango exitoso!");
        FinishAttack();
    }

    private void FinishAttack()
    {
        _agent.CurrentTBATimer = 0f; 
        _agent.FSM.ChangeState(_agent.Patrol); 
    }

    public override void Exit() => Debug.Log("Saliendo de Attack");
}