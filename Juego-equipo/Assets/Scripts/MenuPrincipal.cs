using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // Libreria para el cambio de escena
using Firebase.Auth;
using Firebase.Firestore; // Para Firestore
using Firebase.Extensions; // Asegúrate de incluir esta línea


public class MenuPrincipal : MonoBehaviour
{
    // Referencia a Firestore
    FirebaseFirestore firestore;

    // Método para inicializar Firebase y Firestore
    private void Start()
    {
        // Inicializa Firestore
        firestore = FirebaseFirestore.DefaultInstance;
    }

    public void cambiarEscena()
    {
        // Obtener el UID del usuario actual
        string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        Debug.Log("UID del usuario actual: " + uid); // Log para ver el UID

        // Obtener la referencia al documento del progreso del usuario
        DocumentReference progresoRef = firestore.Collection("ProgresoUsuario").Document(uid);

        progresoRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error al obtener el progreso del usuario: " + task.Exception);
                return;
            }

            var snapshot = task.Result;
            string ultimoNivel = "Nivel1"; // Valor predeterminado

            Debug.Log("Documento obtenido. Existencia: " + snapshot.Exists); // Verifica si el documento existe

            if (snapshot.Exists)
            {
                try
                {
                    // Verificar si el campo 'nivel_id' existe en el documento
                    if (snapshot.ContainsField("nivel_id"))
                    {
                        // Obtener el último nivel completado por el usuario (como cadena)
                        ultimoNivel = snapshot.GetValue<string>("nivel_id");
                        Debug.Log("Último nivel completado por el usuario: " + ultimoNivel);
                    }
                    else
                    {
                        Debug.LogError("El campo 'nivel_id' no se encuentra en el documento de Firestore.");
                    }

                    // Extraer el número del nivel (suponiendo que el formato es 'NivelX')
                    int numeroNivel = int.Parse(ultimoNivel.Replace("Nivel", ""));
                    int siguienteNivel = numeroNivel + 1;

                    // Determinar el siguiente nivel
                    string siguienteNivelStr = "Nivel" + siguienteNivel;
                    Debug.Log("Cargando la escena: " + siguienteNivelStr);

                    // Verificar si la escena existe antes de cargarla
                    if (Application.CanStreamedLevelBeLoaded(siguienteNivelStr))
                    {
                        SceneManager.LoadScene(siguienteNivelStr);
                    }
                    else
                    {
                        Debug.LogError("La escena '" + siguienteNivelStr + "' no se encuentra en los Build Settings.");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error al leer el progreso: " + e.Message);
                }
            }
            else
            {
                Debug.Log("No se encontró progreso para este usuario, comenzando desde el nivel 1.");

                // Crear el documento si no existe
                Dictionary<string, object> progresoNuevo = new Dictionary<string, object>
        {
            { "nivel_id", "Nivel1" } // Asignar el nivel inicial como 'Nivel1'
        };

                progresoRef.SetAsync(progresoNuevo).ContinueWithOnMainThread(setTask =>
                {
                    if (setTask.IsFaulted)
                    {
                        Debug.LogError("Error al crear el documento de progreso: " + setTask.Exception);
                        return;
                    }

                    Debug.Log("Documento de progreso creado exitosamente. Iniciando desde el nivel 1.");
                    SceneManager.LoadScene("Nivel1");
                });
            }
        });
    }

    public void salir()
    {
        // Cerrar sesión de Firebase
        FirebaseAuth.DefaultInstance.SignOut();
        Debug.Log("Sesión cerrada correctamente");

        // Redirigir a la pantalla de inicio de sesión
        SceneManager.LoadScene("FirebaseAuth");  // Asegúrate de que el nombre de la escena sea correcto
    }

}
