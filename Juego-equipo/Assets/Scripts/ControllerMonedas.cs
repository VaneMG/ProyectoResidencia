using System.Collections;
using System.Collections.Generic;
using TMPro; // Aseg�rate de agregar esta librer�a
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System;

public class ControllerMonedas : MonoBehaviour
{
    private static FirebaseFirestore firestore;

    [Header("UI")]
    public TextMeshProUGUI textoMonedas; // Campo p�blico para asignar el TextMeshPro desde el Inspector

    private void Awake()
    {
        if (firestore == null)
        {
            firestore = FirebaseFirestore.DefaultInstance;
        }
    }

    public static void ObtenerMonedasDesdeFirestore(string uid, Action<int> callback)
    {
        DocumentReference monedasRef = firestore.Collection("Monedas").Document(uid);

        monedasRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error al obtener las monedas del usuario en Firestore: " + task.Exception);
                callback?.Invoke(0);
                return;
            }

            int monedasTotales = 0;
            if (task.Result.Exists)
            {
                try
                {
                    monedasTotales = task.Result.GetValue<int>("monedas_totales");
                }
                catch (Exception e)
                {
                    Debug.LogError("Error al leer las monedas: " + e.Message);
                }
            }

            callback?.Invoke(monedasTotales);
        });
    }

    public static void GuardarMonedasEnFirestore(string uid, int cambioDeMonedas, Action callback = null)
    {
        DocumentReference monedasRef = firestore.Collection("Monedas").Document(uid);

        monedasRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error al obtener las monedas: " + task.Exception);
                return;
            }

            int monedasTotales = 0;
            if (task.Result.Exists)
            {
                try
                {
                    monedasTotales = task.Result.GetValue<int>("monedas_totales");
                }
                catch (Exception e)
                {
                    Debug.LogError("Error al leer las monedas: " + e.Message);
                }
            }

            monedasTotales += cambioDeMonedas;

            if (monedasTotales < 0)
            {
                Debug.LogWarning("No hay suficientes monedas para esta operaci�n.");
                return;
            }

            var datosMonedas = new Dictionary<string, object>
        {
            { "monedas_totales", monedasTotales },
            { "ultima_actualizacion", DateTime.UtcNow.ToString("o") }
        };

            monedasRef.SetAsync(datosMonedas).ContinueWithOnMainThread(saveTask =>
            {
                if (saveTask.IsFaulted)
                {
                    Debug.LogError("Error al guardar las monedas: " + saveTask.Exception);
                }
                else
                {
                    Debug.Log($"Monedas actualizadas: {monedasTotales}");
                    callback?.Invoke(); // Llama al callback si se proporciona
                }
            });
        });
    }


    // Nuevo m�todo para actualizar el texto de monedas
    public void ActualizarTextoMonedas(string uid)
    {
        ObtenerMonedasDesdeFirestore(uid, (monedasTotales) =>
        {
            if (textoMonedas != null)
            {
                textoMonedas.text = $"{monedasTotales}";
            }
            else
            {
                Debug.LogWarning("El TextMeshPro no est� asignado.");
            }
        });
    }
}
