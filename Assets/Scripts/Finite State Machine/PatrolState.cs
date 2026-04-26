using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{

    private FSMAgent _agent;
    private PatrolData _data;
    private int _currentIndex = 0;
    float tiempoAcumulado = 0f;
    int segundosEnteros = 0;
    
    public PatrolState(PatrolData data, FSMAgent agent) : base(agent.FSM) 
    {
        _data = data;
        _agent = agent;
    }
    public override void Enter()
    {
        Debug.LogError("Entre a Patrol");
        
    }

    public override void Update()
    {
        Collider[] closeObjects = Physics.OverlapSphere(_agent.transform.position, _agent.ViewRadius);
        foreach (var col in closeObjects)
        {
            if (col.CompareTag(_agent.TargetTagAgent)) 
            {
                Debug.Log("detecto un agente");
                _agent.SetTargetAgent(col.GetComponent<FSMAgent>());
                break;
            }
        }
        Debug.Log("detecto un agente");
        if(_agent.TargetAgent != null)
        {
            Debug.Log("va a perseguir un agente");
            _agent.FSM.ChangeState(_agent.Pursuit);
        }
        else
        {
            Transform nextWaypoint = _data.waypoints[_currentIndex];
            if(Vector3.Distance(nextWaypoint.position, _data.transform.position) <= _data.waypointDistance)
            {
                _currentIndex = _currentIndex + 1 < _data.waypoints.Count ? _currentIndex + 1 : 0;
                nextWaypoint = _data.waypoints[_currentIndex];
            }
            
            Vector3 dir = nextWaypoint.position - _data.transform.position;
            _data.transform.position += _agent.Speed * Time.deltaTime * dir.normalized;
            _data.transform.forward = dir;
        }
        tiempoAcumulado += Time.deltaTime;
        if (tiempoAcumulado >= 1f) {
            segundosEnteros++;
            tiempoAcumulado -= 1f; 
        }
        if(segundosEnteros % 5 == 0)
        {   
            _agent.FSM.ChangeState(_agent.Cheese);
        }
    }
    public override void Exit()
    {
        Debug.LogError("Sali de Patrol");
    }
}

[System.Serializable]
public class PatrolData
{
    public List<Transform> waypoints;
    public Transform transform;
    public float waypointDistance;
}