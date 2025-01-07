using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using TMPro; // Necesario para TextMeshPro


public class SeleccionPersonaje : MonoBehaviour
{
    public Image[] contenedores; // Referencia a los contenedores
    public Sprite defaultImage; // Imagen predeterminada
    public Button[] botones;    // Botones asociados a las im�genes
    public TextMeshProUGUI textoPersonajeSeleccionado; // Referencia al TextMeshPro
    public string[] itemIDs; // Lista de itemIDs que se asignar�n en el Inspector


    public Button ranaButton; // Bot�n de la rana
    public Image ranaContenedor; // Contenedor de la imagen de la rana

    private FirebaseAuth auth;
    private FirebaseFirestore db;

    void Start()
    {
        // Inicializamos Firebase
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        // Obtener el personaje seleccionado al iniciar
        CargarPersonajeSeleccionado();
    }

    void OnEnable()
    {
        // Suscribir al evento de "imagenes listas" cuando el objeto se habilite
        PerfilAvatar.OnImagenesListas += ConfigurarBotones;
    }

    void OnDisable()
    {
        // Asegurarse de desuscribirse del evento cuando el objeto se desactive
        PerfilAvatar.OnImagenesListas -= ConfigurarBotones;
    }

    public void ConfigurarBotones()
    {
        for (int i = 0; i < contenedores.Length; i++)
        {
            if (i < botones.Length)
            {
                // Verificar si la imagen del contenedor es diferente a la predeterminada
                if (contenedores[i].sprite != null && contenedores[i].sprite != defaultImage)
                {
                    botones[i].interactable = true; // Habilitar el bot�n
                }
                else
                {
                    botones[i].interactable = false; // Desactivar el bot�n
                }

                // Eliminar listeners previos para evitar duplicados
                botones[i].onClick.RemoveAllListeners();

                // A�adir l�gica personalizada al bot�n
                int index = i; // Capturar �ndice para usar en el evento
                botones[i].onClick.AddListener(() => OnButtonClick(index));
            }
        }

        // Configurar el bot�n de la rana
        ConfigurarBotonRana();
    }

    private void ConfigurarBotonRana()
    {
        // Verificar si la imagen de la rana est� asignada
        if (ranaContenedor.sprite != null && ranaContenedor.sprite != defaultImage)
        {
            ranaButton.interactable = true; // Habilitar el bot�n si la imagen est� asignada
        }
        else
        {
            ranaButton.interactable = false; // Desactivar si la imagen est� en blanco o es la predeterminada
        }

        ranaButton.onClick.RemoveAllListeners();
        ranaButton.onClick.AddListener(() => OnRanaButtonClick());
    }

    private void OnButtonClick(int index)
    {
        if (index >= 0 && index < itemIDs.Length)
        {
            string personajeId = itemIDs[index];  // Obtener el itemID directamente del arreglo
            Debug.Log($"Bot�n {index} presionado. Seleccionado ID: {personajeId}");
            GuardarSeleccionPersonaje(personajeId);
        }
        else
        {
            Debug.LogError("�ndice fuera de rango en los itemIDs.");
        }

    }

    private void OnRanaButtonClick()
    {
        Debug.Log("Bot�n de Rana presionado. Imagen actual: " + ranaContenedor.sprite.name);
        // Aqu� a�ades la l�gica para manejar la selecci�n del personaje de la rana
        string personajeId = "personaje_rana";  // ID �nico para el personaje de la rana
        GuardarSeleccionPersonaje(personajeId);
    }
    private void VerificarOCrearDocumentoUsuario()
    {
        // Obtener el ID del usuario actual
        string userId = auth.CurrentUser.UserId;

        // Referencia al documento del usuario en Firestore
        DocumentReference userDocRef = db.Collection("Usuarios").Document(userId);

        // Intentar obtener el documento del usuario
        userDocRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;
                if (!snapshot.Exists)
                {
                    // Si el documento no existe, crear uno con datos iniciales
                    userDocRef.SetAsync(new Dictionary<string, object>
                {
                    { "personajeSeleccionado", "ninguno" }, // Valor inicial
                }).ContinueWithOnMainThread(createTask =>
                {
                    if (createTask.IsCompleted)
                    {
                        Debug.Log("Documento del usuario creado en Firebase.");
                    }
                    else
                    {
                        Debug.LogError("Error al crear documento del usuario: " + createTask.Exception);
                    }
                });
                }
                else
                {
                    Debug.Log("El documento del usuario ya existe en Firebase.");
                }
            }
            else
            {
                Debug.LogError("Error al verificar documento del usuario: " + task.Exception);
            }
        });
    }

    private void GuardarSeleccionPersonaje(string personajeId)
    {
        // Verificar o crear el documento del usuario antes de guardar la selecci�n
        VerificarOCrearDocumentoUsuario();

        // Obtener el ID del usuario actual
        string userId = auth.CurrentUser.UserId;

        // Guardar la selecci�n en Firestore
        DocumentReference userDocRef = db.Collection("Usuarios").Document(userId);

        // Actualizar el documento del usuario con el personaje seleccionado
        userDocRef.UpdateAsync("personajeSeleccionado", personajeId).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Personaje seleccionado guardado correctamente en Firebase.");
                // Actualizar el texto en la UI de inmediato
                textoPersonajeSeleccionado.text = $"Personaje Seleccionado: {personajeId}";
            }
            else
            {
                Debug.LogError("Error al guardar personaje en Firebase: " + task.Exception);
            }
        });

    }

    void CargarPersonajeSeleccionado()
    {
        // Verificar o crear el documento del usuario antes de cargar datos
        VerificarOCrearDocumentoUsuario();

        // Obtener el ID del usuario actual
        string userId = auth.CurrentUser.UserId;

        // Referencia al documento del usuario en Firestore
        DocumentReference userDocRef = db.Collection("Usuarios").Document(userId);

        // Obtener los datos del documento
        userDocRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists && snapshot.TryGetValue("personajeSeleccionado", out string personajeSeleccionado))
                {
                    // Actualizar el TextMeshPro con el personaje seleccionado
                    textoPersonajeSeleccionado.text = $"Personaje Seleccionado: {personajeSeleccionado}";
                }
                else
                {
                    textoPersonajeSeleccionado.text = "No hay personaje seleccionado.";
                }
            }
            else
            {
                Debug.LogError("Error al obtener datos de Firebase: " + task.Exception);
                textoPersonajeSeleccionado.text = "Error al cargar personaje.";
            }
        });
    }

}

