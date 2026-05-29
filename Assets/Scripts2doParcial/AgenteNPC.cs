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
    
    private float temporizadorRuta = 0f;
    private List<Node> rutaPersecucion;
    private int indicePersecucion = 0;
    private bool estaAlertadoGlobal = false;
    private Vector3 ultimaPosicionAlerta;

    void Update()
    {
        if (PuedeVerAlJugador())
        {
            if (estadoActual != EstadoNPC.Persiguiendo || !estaAlertadoGlobal)
            {
                estaAlertadoGlobal = true;
                GameManager.Instancia.AlertarAgentes(player.position);
            }
        }

        switch (estadoActual)
        {
            case EstadoNPC.Patrullando:
                EjecutarPatrulla();
                break;

            case EstadoNPC.Persiguiendo:
                EjecutarPersecucion();
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
            direccionAlJugador.y = 0; 

            if (direccionAlJugador != Vector3.zero)
            {
                Quaternion rotacionDeseada = Quaternion.LookRotation(direccionAlJugador);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, velocidadRotacion * Time.deltaTime);
            }

            transform.position = Vector3.MoveTowards(transform.position, player.position, velocidad * Time.deltaTime);
            ultimaPosicionAlerta = player.position;

            temporizadorRuta += Time.deltaTime;
            if (temporizadorRuta >= tiempoRecalculo)
            {
                temporizadorRuta = 0f; 
                RecalcularRutaAStar(player.position);
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
            else
            {
                Vector3 direccionAlDestino = (ultimaPosicionAlerta - transform.position).normalized;
                direccionAlDestino.y = 0;

                if (Vector3.Distance(transform.position, ultimaPosicionAlerta) > 0.5f)
                {
                    if (direccionAlDestino != Vector3.zero)
                    {
                        Quaternion rotacionDeseada = Quaternion.LookRotation(direccionAlDestino);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, velocidadRotacion * Time.deltaTime);
                    }
                    transform.position = Vector3.MoveTowards(transform.position, ultimaPosicionAlerta, velocidad * Time.deltaTime);
                }
                else
                {
                    VolverAPatrullaje();
                }
            }
        }

        if (Vector3.Distance(transform.position, player.position) < 1.2f)
        {
            GameManager.Instancia.Derrota();
        }
    }

    private void RecalcularRutaAStar(Vector3 posicionDestino)
    {
        Node nodoInicio = managerRutas.EncontrarNodoMasCercano(transform.position);
        Node nodoDestino = managerRutas.EncontrarNodoMasCercano(posicionDestino);

        if (nodoInicio != null && nodoDestino != null)
        {
            rutaPersecucion = managerRutas.EncontrarCamino(nodoInicio, nodoDestino);
            
            if (rutaPersecucion != null && rutaPersecucion.Count > 1) indicePersecucion = 1; 
            else indicePersecucion = 0; 
        }
    }

    private void VolverAPatrullaje()
    {
        estadoActual = EstadoNPC.Patrullando;
        estaAlertadoGlobal = false;
        GetComponent<Renderer>().material.color = Color.white; 

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

    private bool PuedeVerAlJugador()
    {
        if (player == null || !player.gameObject.activeSelf) return false;

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

    public void AlertaRecibida(Vector3 posicionInvestigar)
    {
        if (player == null || !player.gameObject.activeSelf) return;

        estadoActual = EstadoNPC.Persiguiendo;
        estaAlertadoGlobal = true;
        GetComponent<Renderer>().material.color = Color.red;

        ultimaPosicionAlerta = posicionInvestigar;
        RecalcularRutaAStar(ultimaPosicionAlerta);
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