using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Necesario para TextMeshPro
using Firebase.Auth; // Asegúrate de tener la librería de autenticación de Firebase
using Firebase.Firestore;
using Firebase.Extensions;
using System;

public class Shop : MonoBehaviour
{
    public bool IsPurchased = false; // Indica si el ítem ha sido comprado
    public Button comprarButton; // Botón para comprar
    private TextMeshProUGUI buttonText; // Texto del botón
    public int precio = 0; // Precio del ítem

    public ControllerMonedas controllerMonedas; // Referencia al script ControllerMonedas

    public Image itemImage; // Imagen del artículo (asignada desde el Inspector)
    public Sprite defaultImage; // Imagen predeterminada si no hay otra asignada
    public string imageID; // Identificador único de la imagen

    private FirebaseAuth auth;
    private FirebaseFirestore db;
    private string itemID; // Identificador único del ítem


    // Start is called before the first frame update
    void Start()
    {
        // Inicializamos Firebase
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        // Inicializar el personaje predeterminado
        InicializarPersonajeRana();

        // Obtener el identificador único del ítem (nombre del GameObject)
        itemID = gameObject.name;

        // Obtener el componente ControllerMonedas si no está asignado
        if (controllerMonedas == null)
        {
            controllerMonedas = FindObjectOfType<ControllerMonedas>();
            if (controllerMonedas == null)
            {
                Debug.LogError("No se encontró un ControllerMonedas en la escena.");
                return;
            }
        }

        // Configurar la imagen predeterminada
        if (itemImage == null)
        {
            itemImage = GetComponentInChildren<Image>();
        }

        if (itemImage.sprite == null && defaultImage != null)
        {
            itemImage.sprite = defaultImage;
        }

        buttonText = comprarButton.GetComponentInChildren<TextMeshProUGUI>();

        if (comprarButton != null)
        {
            buttonText.text = "COMPRAR";
            comprarButton.onClick.AddListener(OnComprarClicked);
        }

        // Cargar el estado de compra desde Firebase
        CargarEstadoDeCompraDesdeFirestore(itemID);

        ActualizarVisualizacion();
    }

    private void OnComprarClicked()
    {
        // Obtener el ID del usuario actual
        string userId = auth.CurrentUser.UserId;
        ControllerMonedas.ObtenerMonedasDesdeFirestore(userId, (monedasTotales) =>
        {
            if (monedasTotales >= precio)
            {
                ControllerMonedas.GuardarMonedasEnFirestore(userId, -precio, () =>
                {
                    // Actualizar el estado de compra
                    IsPurchased = true;
                    comprarButton.interactable = false;
                    buttonText.text = "COMPRADO";

                    // Guardar el estado y el identificador de la imagen en Firebase
                    GuardarEstadoDeCompraEnFirestore(itemID, imageID);

                    ActualizarVisualizacion();
                    Debug.Log($"Compra realizada. Monedas restantes: {monedasTotales - precio}");
                });
            }
            else
            {
                Debug.Log("No tienes suficientes monedas para comprar este objeto.");
            }
        });
    }
    private void InicializarPersonajeRana()
    {
        // Obtener el ID del usuario actual
        string userId = auth.CurrentUser.UserId;

        string defaultItemID = "personaje_rana"; // Identificador del personaje predeterminado
        string defaultImageID = "image_rana";   // Identificador único de la imagen predeterminada

        DocumentReference shopRef = FirebaseFirestore.DefaultInstance.Collection("Tienda").Document(userId);

        shopRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                var data = task.Result.ToDictionary();

                // Verificar si ya existe el personaje predeterminado
                if (!data.ContainsKey(defaultItemID))
                {
                    // Agregar personaje predeterminado como comprado
                    Dictionary<string, object> defaultData = new Dictionary<string, object>
                {
                    { "IsPurchased", true },
                    { "imageID", defaultImageID }
                };

                    shopRef.UpdateAsync(new Dictionary<string, object> { { defaultItemID, defaultData } }).ContinueWithOnMainThread(saveTask =>
                    {
                        if (saveTask.IsCompleted)
                        {
                            Debug.Log("personaje_rana inicializado correctamente.");
                        }
                        else
                        {
                            Debug.LogError("Error al inicializar personaje_rana: " + saveTask.Exception);
                        }
                    });
                }
            }
            else
            {
                Debug.LogError("Error al cargar datos del usuario: " + task.Exception);
            }
        });
    }

    private void CargarEstadoDeCompraDesdeFirestore(string itemID)
    {
        // Obtener el ID del usuario actual
        string userId = auth.CurrentUser.UserId;

        DocumentReference shopRef = FirebaseFirestore.DefaultInstance.Collection("Tienda").Document(userId);

        shopRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error al cargar el estado de compra: " + task.Exception);
                return;
            }

            if (task.Result.Exists && task.Result.ContainsField(itemID))
            {
                Dictionary<string, object> itemData = task.Result.GetValue<Dictionary<string, object>>(itemID);
                if (itemData != null)
                {
                    // Verificar si el artículo ya fue comprado
                    if (itemData.ContainsKey("IsPurchased") && (bool)itemData["IsPurchased"])
                    {
                        IsPurchased = true;
                        comprarButton.interactable = false;
                        buttonText.text = "COMPRADO";

                        // Cargar el identificador de la imagen (si es necesario para mostrarlo en el futuro)
                        if (itemData.ContainsKey("imageID"))
                        {
                            imageID = itemData["imageID"].ToString();
                            Debug.Log($"Se cargó la imagen asociada: {imageID}");
                        }
                    }
                }
            }
            // Si el personaje predeterminado no está en Firebase, inicializarlo
            else if (itemID == "personaje_rana")
            {
                IsPurchased = true;
                comprarButton.interactable = false;
                buttonText.text = "COMPRADO";
            }
        });
    }

    private void GuardarEstadoDeCompraEnFirestore(string itemID, string imageID)
    {
        // Obtener el ID del usuario actual
        string userId = auth.CurrentUser.UserId;

        DocumentReference shopRef = FirebaseFirestore.DefaultInstance.Collection("Tienda").Document(userId);

        shopRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            Dictionary<string, object> compraData = new Dictionary<string, object>();
            if (task.Result.Exists)
            {
                compraData = task.Result.ToDictionary();
            }

            compraData[itemID] = new Dictionary<string, object>
            {
                { "IsPurchased", true },
                { "imageID", imageID } // Guardar el identificador único de la imagen
            };

            shopRef.SetAsync(compraData).ContinueWithOnMainThread(saveTask =>
            {
                if (saveTask.IsFaulted)
                {
                    Debug.LogError("Error al guardar el estado de compra: " + saveTask.Exception);
                }
                else
                {
                    Debug.Log($"Estado de compra actualizado para el ítem: {itemID}, con imagen: {imageID}");
                }
            });
        });
    }

    
    private void ActualizarVisualizacion()
    {
        // Obtener el ID del usuario actual
        string userId = auth.CurrentUser.UserId;
        // Usar el ControllerMonedas para actualizar la UI
        if (controllerMonedas != null)
        {
            controllerMonedas.ActualizarTextoMonedas(userId);
        }
        else
        {
            Debug.LogWarning("El ControllerMonedas no está asignado.");
        }
    }

    private void OnDestroy()
    {
        if (comprarButton != null)
        {
            comprarButton.onClick.RemoveListener(OnComprarClicked);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
