using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; }

    [Header("Referencias")]
    public Transform jugador;
    public Transform puntoSpawn;

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        AsignarPosicionInicial();
    }

    public void AsignarPosicionInicial()
    {
        if (jugador != null && puntoSpawn != null)
        {
            jugador.position = puntoSpawn.position;
        }
    }

    public void Victoria()
    {
        StartCoroutine(SecuenciaVictoria());
    }

    private IEnumerator SecuenciaVictoria()
    {
        
        Debug.Log("¡VICTORIA! Llegaste a la meta con éxito.");

        
        if (jugador != null)
        {
            jugador.gameObject.SetActive(false);
        }

        
        yield return new WaitForSeconds(3f);

        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Derrota()
    {
        StartCoroutine(SecuenciaDerrota());
    }

    private IEnumerator SecuenciaDerrota()
    {
        Debug.Log("¡GAME OVER! Un agente te ha atrapado.");

        if (jugador != null)
        {
            jugador.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(5f);

        if (jugador != null && puntoSpawn != null)
        {
            jugador.position = puntoSpawn.position;
            jugador.gameObject.SetActive(true);
        }

        AgenteNPC[] enemigos = FindObjectsByType<AgenteNPC>(FindObjectsSortMode.None);
        
        foreach (AgenteNPC enemigo in enemigos)
        {
            enemigo.estadoActual = AgenteNPC.EstadoNPC.Patrullando;
            enemigo.GetComponent<Renderer>().material.color = Color.white;
        }
    }
}
