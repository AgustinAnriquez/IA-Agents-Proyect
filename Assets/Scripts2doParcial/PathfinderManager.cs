using UnityEngine;
using System.Collections.Generic;

public class PathfinderManager : MonoBehaviour
{


    private Node[] todosLosNodosDelMapa;
    private Dictionary<Node, Node> padresDeNodos;
    
    //Escanea todo el mapa y guarda en memoria todos los nodos 
    void Awake() 
    {

        todosLosNodosDelMapa = FindObjectsByType<Node>(FindObjectsSortMode.None);
        padresDeNodos = new Dictionary<Node, Node>();
    }

    //Limpia el diccionario y reinicia las distancias 
    private void ReiniciarNodos()
    {
        foreach (Node nodo in todosLosNodosDelMapa)
        {

            nodo.distanceFromStart = Mathf.Infinity;
            nodo.estimatedDistanceToTarget = 0;
        }
        padresDeNodos.Clear();
    }

    //Le dice al nodo inicial en donde esta y al nodo final a donde tiene que llegar
    public List<Node> EncontrarCamino(Node nodoInicio, Node nodoDestino)
    {
        ReiniciarNodos();


    List<Node> openSet = new List<Node>();
    HashSet<Node> closedSet = new HashSet<Node>();


    openSet.Add(nodoInicio);
    

    nodoInicio.distanceFromStart = 0;
    

    nodoInicio.estimatedDistanceToTarget = Vector3.Distance(nodoInicio.transform.position, nodoDestino.transform.position);
    
        while (openSet.Count > 0)
        {
            Node nodoActual = openSet[0];
            
            // Búsqueda del nodo más óptimo
            for (int i = 1; i < openSet.Count; i++)
            {
                float costoFTotal = openSet[i].distanceFromStart + openSet[i].estimatedDistanceToTarget;
                float costoFActual = nodoActual.distanceFromStart + nodoActual.estimatedDistanceToTarget;

                if (costoFTotal < costoFActual || (costoFTotal == costoFActual && openSet[i].estimatedDistanceToTarget < nodoActual.estimatedDistanceToTarget))
                {
                    nodoActual = openSet[i];
                }
            }


            if (nodoActual == nodoDestino)
            {
                return ReconstruirCamino(nodoActual);
            }


            openSet.Remove(nodoActual);
            closedSet.Add(nodoActual);

            
            foreach (Node vecino in nodoActual.nodosConectados)
            {
                if (closedSet.Contains(vecino)) continue;

                float costoNuevoAlVecino = nodoActual.distanceFromStart + Vector3.Distance(nodoActual.transform.position, vecino.transform.position);

                if (costoNuevoAlVecino < vecino.distanceFromStart || !openSet.Contains(vecino))
                {
                    vecino.distanceFromStart = costoNuevoAlVecino;
                    vecino.estimatedDistanceToTarget = Vector3.Distance(vecino.transform.position, nodoDestino.transform.position);
                    padresDeNodos[vecino] = nodoActual;

                    if (!openSet.Contains(vecino))
                    {
                        openSet.Add(vecino);
                    }
                }
            }
        }

    return null; 
    }


    //Lee la informacion usando el diccionario una vez que encuentra el destino 
    private List<Node> ReconstruirCamino(Node nodoFinal)
    {
        List<Node> caminoFinal = new List<Node>();
        Node nodoActual = nodoFinal;
        
        caminoFinal.Add(nodoActual);

        
        while (padresDeNodos.ContainsKey(nodoActual))
        {
            nodoActual = padresDeNodos[nodoActual];
            caminoFinal.Add(nodoActual);
        }

        caminoFinal.Reverse();
        return caminoFinal;
    }
}
