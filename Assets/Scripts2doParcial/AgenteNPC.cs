using UnityEngine;
using System.Collections.Generic;

public class AgenteNPC : MonoBehaviour
{
    public enum EstadoNPC { Patrullando, Persiguiendo }

    [Header("Conexión con la IA")]
    public PathfinderManager managerRutas; 

    [Header("Físicas")]
    public float velocidad = 4f;
    public float velocidadRotacion = 8f;

    [Header("Sistema de Patrullaje")]
    public List<Node> rutaDePatrulla; 
    public Color colorDelCircuito = Color.red;
    private int indicePatrulla = 0;

    [Header("Sentidos (Detección)")]
    public Transform player; 
    public float vistaRadio = 10f; 
    [Range(0, 180)]
    public float vistaAngulo = 90f; 
    private float alturaOjos = 1.5f;

    [Header("Máquina de Estados")]
    public EstadoNPC estadoActual = EstadoNPC.Patrullando;
    public float tiempoRecalculo = 0.5f; 
    public float tiempoParaOlvidar = 3f; 
    
    private float temporizadorOlvido = 0f;
    private float temporizadorRuta = 0f;
    private List<Node> rutaPersecucion;
    private int indicePersecucion = 0;

    void Update()
    {
        switch (estadoActual)
        {
            case EstadoNPC.Patrullando:
                EjecutarPatrulla();

                if (PuedeVerAlJugador())
                {
                    estadoActual = EstadoNPC.Persiguiendo;
                    GetComponent<Renderer>().material.color = Color.red;
                    temporizadorOlvido = 0f; 
                }
                break;

            case EstadoNPC.Persiguiendo:
                EjecutarPersecucion();
                
                if (!PuedeVerAlJugador())
                {
                    temporizadorOlvido += Time.deltaTime;
                    GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0f); 

                    if (temporizadorOlvido >= tiempoParaOlvidar)
                    {
                        estadoActual = EstadoNPC.Patrullando;
                        GetComponent<Renderer>().material.color = Color.white; 
                        temporizadorOlvido = 0f;

                        float distanciaMin = Mathf.Infinity;
                        for (int i = 0; i < rutaDePatrulla.Count; i++)
                        {
                            float dist = Vector3.Distance(transform.position, rutaDePatrulla[i].transform.position);
                            if (dist < distanciaMin)
                            {
                                distanciaMin = dist;
                                indicePatrulla = i; 
                            }
                        }
                    }
                }
                else
                {
                    temporizadorOlvido = 0f;
                    GetComponent<Renderer>().material.color = Color.red;
                }
                break;
        }
    }

    private void EjecutarPatrulla()
    {
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

    private void EjecutarPersecucion()
    {
        
        if (PuedeVerAlJugador())
        {
            Vector3 direccionAlJugador = (player.position - transform.position).normalized;
            direccionAlJugador.y = 0; // Bloquea cabeceos extraños en rampas

            if (direccionAlJugador != Vector3.zero)
            {
                Quaternion rotacionDeseada = Quaternion.LookRotation(direccionAlJugador);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, velocidadRotacion * Time.deltaTime);
            }

            
            transform.position = Vector3.MoveTowards(transform.position, player.position, velocidad * Time.deltaTime);

            
            temporizadorRuta += Time.deltaTime;
            if (temporizadorRuta >= tiempoRecalculo)
            {
                temporizadorRuta = 0f; 
                Node nodoInicio = managerRutas.EncontrarNodoMasCercano(transform.position);
                Node nodoDestino = managerRutas.EncontrarNodoMasCercano(player.position);

                if (nodoInicio != null && nodoDestino != null)
                {
                    rutaPersecucion = managerRutas.EncontrarCamino(nodoInicio, nodoDestino);
                    
                    if (rutaPersecucion != null && rutaPersecucion.Count > 1) indicePersecucion = 1; 
                    else indicePersecucion = 0; 
                }
            }
        }
        
        else 
        {
            if (rutaPersecucion != null && indicePersecucion < rutaPersecucion.Count)
            {
                Node nodoObjetivo = rutaPersecucion[indicePersecucion];
                Vector3 direccionAlObjetivo = (nodoObjetivo.transform.position - transform.position).normalized;
                direccionAlObjetivo.y = 0;
                
                if (direccionAlObjetivo != Vector3.zero)
                {
                    Quaternion rotacionDeseada = Quaternion.LookRotation(direccionAlObjetivo);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, velocidadRotacion * Time.deltaTime);
                }

                transform.position = Vector3.MoveTowards(transform.position, nodoObjetivo.transform.position, velocidad * Time.deltaTime);

                if (Vector3.Distance(transform.position, nodoObjetivo.transform.position) < 0.1f)
                {
                    indicePersecucion++; 
                }
            }
        }

        
        if (Vector3.Distance(transform.position, player.position) < 1.2f)
        {
            GameManager.Instancia.Derrota();
        }
    }

    private bool PuedeVerAlJugador()
    {
        if (player == null) return false;

        Vector3 posicionOjosNPC = transform.position + Vector3.up * alturaOjos;
        Vector3 posicionCentroJugador = player.position + Vector3.up * 1f;
        Vector3 dirAlJugador = posicionCentroJugador - posicionOjosNPC;
        float distanciaAlJugador = dirAlJugador.magnitude;

        if (distanciaAlJugador > vistaRadio) return false; 

        float anguloAlJugador = Vector3.Angle(transform.forward, dirAlJugador);

        if (anguloAlJugador < vistaAngulo / 2f)
        {
            if (Physics.Raycast(posicionOjosNPC, dirAlJugador.normalized, out RaycastHit hit, vistaRadio))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true; 
                }
            }
        }
        return false;
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

        Vector3 posicionOjos = transform.position + Vector3.up * alturaOjos;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(posicionOjos, vistaRadio);

        Vector3 limiteDerecho = Quaternion.Euler(0, vistaAngulo / 2f, 0) * transform.forward;
        Vector3 limiteIzquierdo = Quaternion.Euler(0, -vistaAngulo / 2f, 0) * transform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(posicionOjos, limiteDerecho * vistaRadio);
        Gizmos.DrawRay(posicionOjos, limiteIzquierdo * vistaRadio);
    }
}