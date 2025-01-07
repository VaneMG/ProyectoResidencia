using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;
using UnityEngine.EventSystems;


public class CargaPersonaje : MonoBehaviour
{
    public GameObject jugador; // El objeto jugador que se va a reemplazar
    public GameObject jugadorRana; // El prefab de la rana
    public GameObject jugadorApache; // El prefab del apache (puedes añadir más personajes según sea necesario)
    public GameObject jugadorFinn;
    public GameObject jugadorAstronauta;
    public GameObject jugadorOso;

    private FirebaseFirestore db;
    private FirebaseAuth auth;

    void Start()
    {
        // Inicializar Firebase
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;

        // Cargar el personaje seleccionado desde Firebase
        CargarPersonajeDesdeFirebase();
    }

    void CargarPersonajeDesdeFirebase()
    {
        // Obtener el ID del usuario actual
        string userId = auth.CurrentUser.UserId;

        // Obtener la referencia al documento del usuario en Firestore
        DocumentReference userDocRef = db.Collection("Usuarios").Document(userId);

        // Obtener los datos del usuario
        userDocRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                // Obtener el ID del personaje seleccionado desde la base de datos
                var personajeId = task.Result.GetValue<string>("personajeSeleccionado");

                // Cargar el prefab del personaje basado en el ID
                CargarPrefabPersonaje(personajeId);
            }
            else
            {
                Debug.LogError("No se encontró el documento del usuario o el personaje seleccionado.");
            }
        });
    }

    void CargarPrefabPersonaje(string personajeId)
    {
        GameObject prefabSeleccionado = null;

        // Seleccionar el prefab correcto según el ID del personaje
        switch (personajeId)
        {
            case "personaje_rana":
                prefabSeleccionado = jugadorRana;
                break;
            case "personaje_Apache":
                prefabSeleccionado = jugadorApache;
                break;
            case "personaje_finn":
                prefabSeleccionado = jugadorFinn;
                break;
            case "personaje_astronauta":
                prefabSeleccionado = jugadorAstronauta;
                break;
            case "personaje_Oso":
                prefabSeleccionado = jugadorOso;
                break;
            default:
                Debug.LogWarning("Personaje no encontrado, usando predeterminado.");
                break;
        }

        if (prefabSeleccionado != null)
        {
            // Guardar posición y rotación del jugador actual
            Vector3 posicionJugador = jugador.transform.position;
            Quaternion rotacionJugador = jugador.transform.rotation;

            // Instanciar el nuevo prefab
            GameObject nuevoJugador = Instantiate(prefabSeleccionado, posicionJugador, rotacionJugador);

            // Asignar el tag "Player" al nuevo objeto
            nuevoJugador.tag = "Player";

            // Destruir el jugador actual
            Destroy(jugador);

            // Actualizar la referencia del jugador
            jugador = nuevoJugador;

            // Asignar GameOver al script vidasPlay del nuevo jugador
            var vidasScript = nuevoJugador.GetComponent<vidasPlay>();
            if (vidasScript != null)
            {
                var gameOverManager = FindObjectOfType<GameOver>();
                if (gameOverManager != null)
                {
                    vidasScript.gameOverManager = gameOverManager; // Asignar la referencia
                }
                else
                {
                    Debug.LogError("No se encontró el GameOverManager en la escena.");
                }
            }


            // Actualizar referencias en scripts dependientes
            ActualizarReferencias(nuevoJugador);
        }

    }

    void ActualizarReferencias(GameObject nuevoJugador)
    {
        // Actualizar en ContraDatosPlayer
        var contraDatos = FindObjectOfType<ContraDatosPlayer>();
        if (contraDatos != null)
        {
            contraDatos.jugador = nuevoJugador;
            contraDatos.AsignarVidaJugador(nuevoJugador.GetComponent<vidasPlay>());
        }
        // Actualizar en CorazonesUI
        var corazonesUI = FindObjectOfType<CorazonesUI>();
        if (corazonesUI != null)
        {
            // Desregistrar el evento anterior para evitar conflictos
            if (corazonesUI.VidasPlay != null)
            {
                corazonesUI.VidasPlay.cambioVida.RemoveListener(corazonesUI.RecibirCambioDeVida);
            }

            // Asignar el nuevo script de vidasPlay
            corazonesUI.VidasPlay = nuevoJugador.GetComponent<vidasPlay>();

            // Registrar el nuevo evento
            if (corazonesUI.VidasPlay != null)
            {
                corazonesUI.VidasPlay.cambioVida.AddListener(corazonesUI.RecibirCambioDeVida);
            }
            else
            {
                Debug.LogError("El nuevo jugador no tiene un componente de vidasPlay.");
            }
        }


        // Actualizar EventTrigger de los botones en el Canvas
        ActualizarBotonesCanvas(nuevoJugador);

        // Actualizar en SeguimientoCamara
        var seguimientoCamara = FindObjectOfType<SeguimientoCamara>();
        if (seguimientoCamara != null)
        {
            seguimientoCamara.seguir = nuevoJugador; // Actualizar la referencia del jugador
        }

        // Actualizar cualquier otro script relacionado
        var playController = nuevoJugador.GetComponent<PlayController>();
        if (playController != null)
        {
            Debug.Log("PlayController correctamente asignado al nuevo jugador.");
        }
        else
        {
            Debug.LogError("El prefab no tiene el script PlayController asignado.");
        }
    }

    void ActualizarBotonesCanvas(GameObject nuevoJugador)
    {
        var canvasTeclados = GameObject.Find("Teclados");
        if (canvasTeclados == null)
        {
            Debug.LogError("No se encontró el Canvas 'Teclados' en la escena.");
            return;
        }

        // Encontrar el componente PlayerController del nuevo jugador
        var playerController = nuevoJugador.GetComponent<PlayController>();
        if (playerController == null)
        {
            Debug.LogError("El nuevo jugador no tiene el script PlayerController.");
            return;
        }

        // Actualizar el botón leftButton
        var leftButton = canvasTeclados.transform.Find("leftButton")?.GetComponent<EventTrigger>();
        if (leftButton != null)
        {
            leftButton.triggers.Clear();

            // Asignar evento PointerDown para clickLeft
            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            entry.callback.AddListener((data) => { playerController.clickLeft(); });
            leftButton.triggers.Add(entry);

            // Asignar evento PointerUp para relaseLeft
            entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            entry.callback.AddListener((data) => { playerController.relaseLeft(); });
            leftButton.triggers.Add(entry);
        }
        else
        {
            Debug.LogError("No se encontró el botón 'leftButton' dentro del Canvas 'Teclados'.");
        }

        // Actualizar el botón rigthButton
        var rigthButton = canvasTeclados.transform.Find("rigthButton")?.GetComponent<EventTrigger>();
        if (rigthButton != null)
        {
            rigthButton.triggers.Clear();

            // Asignar evento PointerDown para clickRigth
            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            entry.callback.AddListener((data) => { playerController.clickRigth(); });
            rigthButton.triggers.Add(entry);

            // Asignar evento PointerUp para relaseRigth
            entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            entry.callback.AddListener((data) => { playerController.relaseRigth(); });
            rigthButton.triggers.Add(entry);
        }
        else
        {
            Debug.LogError("No se encontró el botón 'rigthButton' dentro del Canvas 'Teclados'.");
        }

        // Actualizar el botón upButton
        var upButton = canvasTeclados.transform.Find("upButton")?.GetComponent<EventTrigger>();
        if (upButton != null)
        {
            upButton.triggers.Clear();

            // Asignar evento PointerDown para clickJump
            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            entry.callback.AddListener((data) => { playerController.clickJump(); });
            upButton.triggers.Add(entry);
        }
        else
        {
            Debug.LogError("No se encontró el botón 'upButton' dentro del Canvas 'Teclados'.");
        }

        Debug.Log("Referencias de los botones actualizadas para el nuevo jugador.");
    }

}
