using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgenteNPC : MonoBehaviour
{
    [Header("Conexión con el Backend")]
    
    public PathfinderManager managerRutas; 
    
    
    public Node nodoInicioPrueba; 
    public Node nodoDestinoPrueba;

    [Header("Físicas")]
    public float velocidad = 4f;

    // Estado interno
    private List<Node> rutaActual;
    private int indiceDeRuta = 0;

    IEnumerator Start()
    {
        
        yield return new WaitForSeconds(0.5f);

        
        rutaActual = managerRutas.EncontrarCamino(nodoInicioPrueba, nodoDestinoPrueba);

        
        if (rutaActual != null && rutaActual.Count > 0)
        {
            Debug.Log("¡Ruta calculada con éxito! Pasos totales: " + rutaActual.Count);
        }
        else
        {
            Debug.LogWarning("El Manager devolvió null o una lista vacía. Ruta imposible.");
        }
    }

    void Update()
    {
        
        MoverPorLaRuta();
    }

    private void MoverPorLaRuta()
    {
        
        if (rutaActual == null || indiceDeRuta >= rutaActual.Count)
        {
            return; 
        }

        
        Node nodoObjetivo = rutaActual[indiceDeRuta];

        
        transform.position = Vector3.MoveTowards(transform.position, nodoObjetivo.transform.position, velocidad * Time.deltaTime);

        
        if (Vector3.Distance(transform.position, nodoObjetivo.transform.position) < 0.1f)
        {
            indiceDeRuta++; 
        }
    }
}