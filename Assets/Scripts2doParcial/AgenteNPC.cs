using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgenteNPC : MonoBehaviour
{
    [Header("Conexión con la IA")]
    
    public PathfinderManager managerRutas; 

    [Header("Físicas")]
    public float velocidad = 4f;
    public float velocidadRotacion = 8f;

    [Header("Sistema de Patrullaje")]
    public List<Node> rutaDePatrulla; 
    public Color colorDelCircuito = Color.red;
    private List<Node> rutaActual;
    private int indicePatrulla = 0;

    [Header("Sentidos (Detección)")]
    public Transform player; 
    public float vistaRadio = 10f; 
    [Range(0, 180)]
    public float vistaAngulo = 90f; 

    private float alturaOjos = 1.5f;


    void Update()
    {
        
        EjecutarPatrulla();

        if (PuedeVerAlJugador())
    {
        
        GetComponent<Renderer>().material.color = Color.red;
        Debug.Log("¡TE VI!"); 
    }
    else
    {

        GetComponent<Renderer>().material.color = Color.white;
    }
    }

    private void EjecutarPatrulla()
    {
        // Guardia de seguridad
        if (rutaDePatrulla == null || rutaDePatrulla.Count == 0) return;

        Node nodoObjetivo = rutaDePatrulla[indicePatrulla];

        
        Vector3 direccionAlObjetivo = (nodoObjetivo.transform.position - transform.position).normalized;
        
        
        if (direccionAlObjetivo != Vector3.zero)
        {
            Quaternion rotacionDeseada = Quaternion.LookRotation(direccionAlObjetivo);
            
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, velocidadRotacion * Time.deltaTime);
        }

        transform.position = Vector3.MoveTowards(transform.position, nodoObjetivo.transform.position, velocidad * Time.deltaTime);

        if (Vector3.Distance(transform.position, nodoObjetivo.transform.position) < 0.1f)
        {
            indicePatrulla = (indicePatrulla + 1) % rutaDePatrulla.Count;
        }
    }

    void OnDrawGizmos()
    {
        if (rutaDePatrulla != null && rutaDePatrulla.Count > 1)
        {
            Gizmos.color = colorDelCircuito;

            for (int i = 0; i < rutaDePatrulla.Count; i++)
            {
                Node nodoActual = rutaDePatrulla[i];
                
                
                Node nodoSiguiente = rutaDePatrulla[(i + 1) % rutaDePatrulla.Count]; 

                if (nodoActual != null && nodoSiguiente != null)
                {
                    
                    Gizmos.DrawLine(nodoActual.transform.position, nodoSiguiente.transform.position);
                    
                    Gizmos.DrawSphere(nodoActual.transform.position, 0.3f);
                }
            }
        }
    }


private bool PuedeVerAlJugador()
    {

        if (player == null) return false;

        Vector3 posicionOjosNPC = transform.position + Vector3.up * alturaOjos;

        Vector3 posicionCentroJugador = player.position + Vector3.up * 1f;


        Vector3 dirAlJugador = posicionCentroJugador - posicionOjosNPC;
        float distanciaAlJugador = dirAlJugador.magnitude;

        if (distanciaAlJugador > vistaRadio)
        {
            return false; 
        }


        float anguloAlJugador = Vector3.Angle(transform.forward, dirAlJugador);


        if (anguloAlJugador < vistaAngulo / 2f)
        {

            if (Physics.Raycast(posicionOjosNPC, dirAlJugador.normalized, out RaycastHit hit, vistaRadio))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.DrawRay(posicionOjosNPC, dirAlJugador.normalized * hit.distance, Color.green); 
                    return true; 
                }
                else
                {
                    Debug.DrawRay(posicionOjosNPC, dirAlJugador.normalized * hit.distance, Color.cyan); 
                }
            }
        }

        return false;
    }
}