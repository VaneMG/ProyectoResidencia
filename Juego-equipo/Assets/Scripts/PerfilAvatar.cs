using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth; // Librería de autenticación Firebase
using Firebase.Firestore; // Librería Firestore
using Firebase.Extensions;
public class PerfilAvatar : MonoBehaviour
{
    [System.Serializable]
    public class ImageMapping
    {
        public string imageID; // Identificador de la imagen
        public Sprite imageSprite; // Imagen asociada
    }
    public delegate void ImagenesListasHandler(); // Delegado para notificar cuando las imágenes están listas
    public static event ImagenesListasHandler OnImagenesListas; // Evento que será llamado cuando las imágenes se hayan cargado
    public Image[] contenedores; // Referencia a los contenedores en blanco
    public Sprite defaultImage; // Imagen predeterminada si no se encuentra una imagen válida
    public List<ImageMapping> imageMappings; // Lista de mapeos (imageID -> Sprite)

    private Dictionary<string, Sprite> imageDictionary; // Diccionario para búsquedas rápidas
    private FirebaseAuth auth;
    private string userUID;

    void Start()
    {
        // Inicializar Firebase Auth
        auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;

        if (user != null)
        {
            userUID = user.UserId;
            Debug.Log("UID del usuario: " + userUID);

            // Inicializar el diccionario de imágenes
            InicializarImageDictionary();

            // Cargar las imágenes compradas del usuario
            CargarComprasDesdeFirestore();
        }
        else
        {
            Debug.LogError("No se encontró un usuario autenticado.");
        }
    }

    private void InicializarImageDictionary()
    {
        imageDictionary = new Dictionary<string, Sprite>();

        foreach (var mapping in imageMappings)
        {
            if (!imageDictionary.ContainsKey(mapping.imageID))
            {
                imageDictionary[mapping.imageID] = mapping.imageSprite;
            }
        }
    }

    private void CargarComprasDesdeFirestore()
    {
        DocumentReference shopRef = FirebaseFirestore.DefaultInstance.Collection("Tienda").Document(userUID);

        shopRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error al cargar las compras: " + task.Exception);
                return;
            }

            if (task.Result.Exists)
            {
                Dictionary<string, object> comprasData = task.Result.ToDictionary();
                int index = 0;

                foreach (var item in comprasData)
                {
                    if (index >= contenedores.Length) break; // Solo mostrar en los contenedores disponibles

                    // Verificar si el ítem tiene una imagen asociada
                    if (item.Value is Dictionary<string, object> itemData &&
                        itemData.ContainsKey("imageID") &&
                        itemData.ContainsKey("IsPurchased") &&
                        (bool)itemData["IsPurchased"])
                    {
                        string imageID = itemData["imageID"].ToString();

                        // Buscar la imagen en el diccionario
                        if (imageDictionary.TryGetValue(imageID, out Sprite itemSprite))
                        {
                            contenedores[index].sprite = itemSprite;
                        }
                        else
                        {
                            Debug.LogWarning($"No se encontró la imagen para el ID: {imageID}. Usando imagen predeterminada.");
                            contenedores[index].sprite = defaultImage;
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"El ítem {item.Key} no tiene una imagen válida asociada.");
                        contenedores[index].sprite = defaultImage;
                    }

                    index++;
                }

                // Rellenar los contenedores restantes con la imagen predeterminada
                for (int i = index; i < contenedores.Length; i++)
                {
                    contenedores[i].sprite = defaultImage;
                }

                // Cuando todas las imágenes están cargadas y asignadas, se invoca el evento
                OnImagenesListas?.Invoke();
            }
            else
            {
                Debug.LogWarning("No se encontró un documento de compras para este usuario.");
            }
        });
    }
}
