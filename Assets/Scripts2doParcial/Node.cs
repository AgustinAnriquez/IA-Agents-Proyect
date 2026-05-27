using UnityEngine;
using System.Collections.Generic;
public class Node : MonoBehaviour
{
    
    public List<Node> nodosConectados = new List<Node>();
    public float radioDeBusqueda = 5f;
    public LayerMask capaMuros;
    [HideInInspector] public float distanceFromStart; //Representa el puntaje de distancia real recorrida 
    [HideInInspector] public float estimatedDistanceToTarget; //Almacena la estimación en línea recta desde este nodo hasta el destino

void Start()
    {
        Collider[] objetosCercanos = Physics.OverlapSphere(transform.position, radioDeBusqueda);

        foreach (Collider obj in objetosCercanos)
        {
            Node nodoEncontrado = obj.GetComponent<Node>();

            if (nodoEncontrado != null && nodoEncontrado != this)
            {
                
                Debug.Log("¡Radar detectó a: " + nodoEncontrado.gameObject.name + "! Tirando rayo...");

                Vector3 direccionAlNodo = nodoEncontrado.transform.position - transform.position;
                float distanciaAlNodo = Vector3.Distance(transform.position, nodoEncontrado.transform.position);

                if (!Physics.Raycast(transform.position, direccionAlNodo, distanciaAlNodo, capaMuros))
                {
                    
                    Debug.Log("¡Conexión exitosa con: " + nodoEncontrado.gameObject.name + "!");
                    nodosConectados.Add(nodoEncontrado);
                }
                else 
                {
                    
                    Debug.LogWarning("El rayo hacia " + nodoEncontrado.gameObject.name + " chocó contra un muro o el piso.");
                }
            }
        }
    }
    public float finalScore
    { 
        get { return distanceFromStart + estimatedDistanceToTarget; } 
    }

    
    [HideInInspector] public Node nodoPadre;

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.2f, 0.2f, 0.5f); 
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}
