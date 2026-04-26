
using UnityEngine;

public class CheeseState : State
{
    private FSMAgent _agent;
    private GameObject _cheesePrefab; 

    public CheeseState(GameObject cheesePrefab, FSMAgent agent) : base(agent.FSM) 
    {
        _cheesePrefab = cheesePrefab;
        _agent = agent;
    }

    public override void Enter()
    {

        if (_cheesePrefab != null)
        {
            Debug.Log("El agente coloco el queso");
            GameObject[] cheeses = GameObject.FindGameObjectsWithTag("InterestObject");
            if(cheeses.Length <= 5) Object.Instantiate(_cheesePrefab, _agent.transform.position, Quaternion.identity);
            Debug.Log("Queso creado en la posición del agente");
            _agent.FSM.ChangeState(_agent.Patrol);
        }
        else
        {
            Debug.LogError("No se asignó un prefab de queso al CheeseState");
        }
    }

    public override void Update() { }

    public override void Exit()
    {
        Debug.Log("El agente coloco el queso");
    }
}
