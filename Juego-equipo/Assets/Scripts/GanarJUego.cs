using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Firebase.Auth; // Asegúrate de incluir este espacio de nombres

public class GanarJuego : MonoBehaviour
{
    public static GanarJuego Instance { get; private set; }

    public GameObject winPanel; // El panel de "WinPlayer"
    public GameObject tiempoRelog;
    public GameObject canvasVida;
    public GameObject seccionMenu;
    public GameObject canva;
    public GameObject visualizarPuntaje;

    public TextMeshProUGUI puntosText; // El TextMeshPro que mostrará los puntos
    public TextMeshProUGUI tiempoText; // Para mostrar el tiempo de juego

    [SerializeField]
    public TextMeshProUGUI mensajeText;
    private string mensajeAlJugador; // Variable para almacenar el mensaje.


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Asegurarse de que solo haya una instancia
        }
    }

    private void Start()
    {
        // Verificar si los objetos están asignados y activarlos/desactivarlos según sea necesario
        if (winPanel != null) winPanel.SetActive(false);
        if (tiempoRelog != null) tiempoRelog.SetActive(true);
        if (canvasVida != null) canvasVida.SetActive(true);
        if (seccionMenu != null) seccionMenu.SetActive(true);
        if (canva != null) canva.SetActive(true);
        if (visualizarPuntaje != null) visualizarPuntaje.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica si el objeto que colisiona tiene un Rigidbody2D
        if (other.attachedRigidbody != null)
        {
            // Verifica si es el jugador buscando el componente 'vidasPlay'
            vidasPlay vidaJugador = other.attachedRigidbody.GetComponent<vidasPlay>();

            if (vidaJugador != null)
            {
                Debug.Log("¡El jugador tocó la bandera!");
                ocultarComponentes();

                // Lógica adicional: guardar datos, detener reloj, etc.
                Reloj reloj = FindObjectOfType<Reloj>();
                if (reloj != null)
                {
                    reloj.GanarJuego();
                    float tiempoFinal = reloj.tiempoFinal; // Obtener el tiempo final
                    Debug.Log("Tiempo final del jugador: " + tiempoFinal);

                    // Mostrar el panel de ganar con el tiempo final correcto
                    MostrarWinPlayer(tiempoFinal);
                }

                string userId = ObtenerUserId(); // Obtenemos el UID
                if (userId != null && ContraDatosPlayer.Instance != null)
                {
                    ContraDatosPlayer.Instance.GuardarDatos(userId);
                }
                else
                {
                    Debug.LogError("No se pudo guardar los datos: UID o instancia de ContraDatosPlayer es nulo.");
                }
            }
        }
    }

    public void ActualizarMensaje(string mensaje)
    {
        mensajeAlJugador = mensaje; // Guarda el mensaje.
        if (mensajeText != null)
        {
            mensajeText.text = mensaje; // Muestra el mensaje inmediatamente si es necesario.
        }
        else
        {
            Debug.LogError("El mensaje no se pudo actualizar porque mensajeText no está asignado.");
        }
    }

    // Método para obtener el mensaje almacenado.
    public string ObtenerMensaje()
    {
        return mensajeAlJugador;
    }


    void MostrarWinPlayer(float tiempoDeJuego)
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);

            if (PuntosGuargar.Instance != null)
            {
                int puntos = PuntosGuargar.Instance.GetPuntaje();
                puntosText.text = "Puntaje: " + puntos.ToString() + " puntos ";
            }
            else
            {
                Debug.LogError("PuntosGuargar Instance es nulo");
            }

            Reloj reloj = FindObjectOfType<Reloj>();
            if (reloj != null)
            {
                float tiempoFinal = reloj.tiempoFinal;

                Debug.Log("Tiempo final de juego: " + tiempoFinal);
            }
            else
            {
                Debug.LogError("No se encontró el reloj.");
            }

            // Mostrar tiempo de juego
            if (tiempoText != null)
            {
                int minutos = Mathf.FloorToInt(tiempoDeJuego / 60F);
                int segundos = Mathf.FloorToInt(tiempoDeJuego % 60F);
                tiempoText.text = "Tiempo de juego: " + minutos.ToString("00") + ":" + segundos.ToString("00");
            }
            else
            {
                Debug.LogError("El componente tiempoText no está asignado.");
            }

            // Mostrar mensaje recibido desde ContraDatosPlayer.
            if (mensajeText != null)
            {
                mensajeText.text = ObtenerMensaje();
            }
            else
            {
                Debug.LogError("El componente mensajeText no está asignado.");
            }
        }
        else
        {
            Debug.LogError("WinPanel no está asignado.");
        }

    }


    void ocultarComponentes()
    {
        if (tiempoRelog != null) tiempoRelog.SetActive(false);
        if (canvasVida != null) canvasVida.SetActive(false);
        if (seccionMenu != null) seccionMenu.SetActive(false);
        if (canva != null) canva.SetActive(false);
        if (visualizarPuntaje != null) visualizarPuntaje.SetActive(false);
    }

    public void Submenus()
    {
        SceneManager.LoadScene("Submenu");
    }

    private string ObtenerUserId()
    {
        // Verificar si hay un usuario autenticado
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            // Retornar el UID del usuario autenticado
            return FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        }
        else
        {
            Debug.LogError("No hay usuario autenticado.");
            return null; // Retornar null o manejar el caso cuando no hay usuario autenticado
        }
    }
}