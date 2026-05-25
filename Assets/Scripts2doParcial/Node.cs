using UnityEngine;
using System.Collections.Generic;
public class Node : MonoBehaviour
{
    
    public List<Node> nodosConectados = new List<Node>();
    public float radioDeBusqueda = 5f;
    public LayerMask capaMuros;
    [HideInInspector] public float distanceFromStart; //Representa el puntaje de distancia real recorrida 
    [HideInInspector] public float estimatedDistanceToTarget; //Almacena la estimación en línea recta desde este nodo hasta el destino


    public float finalScore
    { 
        get { return distanceFromStart + estimatedDistanceToTarget; } 
    }

    
    [HideInInspector] public Node nodoPadre;
}
