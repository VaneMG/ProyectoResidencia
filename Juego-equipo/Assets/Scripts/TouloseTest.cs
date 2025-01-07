using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Para usar TextMeshPro
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using Firebase.Firestore; // Firestore para guardar datos
using Firebase.Auth; // Firebase Auth para la autenticaci�n

public class ToulouseTest : MonoBehaviour
{
    // Variables para conteo
    private int correctSelectedCount = 0; // Variable A
    private int incorrectSelectedCount = 0; // Variable E
    private int correctNotSelectedCount = 0; // Variable O
    private int totalSelectedCount = 0; // Variable R

    // Lista de botones correctos
    private HashSet<string> correctButtons = new HashSet<string>
    {
        "S11", "S34", "S48", "S63", "S84", "S8", "S106", "S118", "S102",
        "S123", "S22", "S69", "S90", "S64", "S113", "S1", "S51", "S28", "S15",
        "S27", "S38", "S45", "S56", "S78", "S74", "S96", "S100", "S117", "S80",
        "S4", "S58", "S76", "S25", "S82", "S40", "S93", "S46", "S32", "S9",
        "S87", "S111", "S19", "S43", "S61", "S99"
    };

    // Lista de botones seleccionados
    private HashSet<string> selectedButtons = new HashSet<string>();

    // Referencias a UI
    public GameObject notificationPanel; // El panel de notificaci�n
    public TextMeshProUGUI notificationTitle; // El t�tulo en el panel
    public TextMeshProUGUI notificationMessage; // El mensaje en el panel

    // Variables para el temporizador
    private float totalTime = 600f; // 10 minutos en segundos
    private float timeRemaining;
    public TMP_Text timerText; // Referencia al TextMeshPro para mostrar el temporizador
    public Button[] symbolButtons; // Referencia a los botones de s�mbolo
    public Button enviarButton; // Referencia al bot�n "Enviar"
    public Button closeButton; // Referencia al bot�n de cerrar

    // Referencia a Firestore y Firebase Auth
    private FirebaseFirestore db;
    private FirebaseAuth auth;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance; // Inicializa Firestore
        auth = FirebaseAuth.DefaultInstance; // Inicializa Firebase Auth
        ResetTimer(); // Inicializar el temporizador al comenzar
        SetSymbolButtonsInteractable(true); // Habilitar botones de s�mbolos al inicio

        // Asignar eventos de clic a cada bot�n de s�mbolo
        foreach (Button button in symbolButtons)
        {
            string buttonName = button.name; // Obt�n el nombre del bot�n
            button.onClick.AddListener(() => OnSymbolButtonClick(buttonName, button));
        }

        // Asignar evento de clic al bot�n de cerrar
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    // M�todo para manejar el clic del bot�n de cerrar
    public void OnCloseButtonClick()
    {
        Debug.Log("Cerrando y regresando al men� principal...");
        SceneManager.LoadScene("MenuPrincipal 1"); // Cambiar "MenuPrincipal" por el nombre real de tu escena
    }

    // M�todo para ser llamado al hacer clic en un bot�n de s�mbolo
    public void OnSymbolButtonClick(string buttonName, Button button)
    {
        if (timeRemaining <= 0)
        {
            Debug.Log("El tiempo se ha agotado. Ya no puedes seleccionar m�s s�mbolos.");
            return; // Salir si el tiempo se ha agotado
        }

        if (selectedButtons.Contains(buttonName))
        {
            // Deseleccionar el bot�n
            selectedButtons.Remove(buttonName);
            totalSelectedCount--;

            if (correctButtons.Contains(buttonName))
            {
                correctSelectedCount--; // Reducir los correctos seleccionados
                Debug.Log(buttonName + " ha sido deseleccionado (correcto).");
            }
            else
            {
                incorrectSelectedCount--; // Reducir los incorrectos seleccionados
                Debug.Log(buttonName + " ha sido deseleccionado (incorrecto).");
            }

            button.image.color = Color.white; // Volver al color original
        }
        else
        {
            if (selectedButtons.Count >= 126)
            {
                Debug.Log("Ya has seleccionado 3 s�mbolos. No puedes seleccionar m�s.");
                return; // Limitar a 3 selecciones
            }
            // Agregar el bot�n a la lista de seleccionados
            selectedButtons.Add(buttonName);
            totalSelectedCount++; // Incrementar el total de seleccionados

            if (correctButtons.Contains(buttonName))
            {
                correctSelectedCount++; // Incrementar los correctos seleccionados
                Debug.Log(buttonName + " es correcto.");
            }
            else
            {
                incorrectSelectedCount++; // Incrementar los incorrectos seleccionados
                Debug.Log(buttonName + " es incorrecto.");
            }

            button.image.color = Color.green; // Cambiar el color del bot�n para indicar selecci�n
        }
    }

    // M�todo para ser llamado al hacer clic en el bot�n "Enviar"
    public void OnEnviarButtonClick()
    {
        // Detener el temporizador al hacer clic en enviar
        CancelInvoke(nameof(UpdateTimer));

        // Desactivar los botones de s�mbolos para evitar m�s selecciones
        SetSymbolButtonsInteractable(false);

        Debug.Log("Enviando resultados...");
        CalculateNotSelected(); // Calcular los correctos no seleccionados
        UpdateCounts(); // Mostrar los resultados
        CalculateIndices(); // Calcular IGAP e ICI
    }

    // M�todo para calcular los correctos no seleccionados
    private void CalculateNotSelected()
    {
        correctNotSelectedCount = correctButtons.Count - correctSelectedCount; // Variable O
    }

    // M�todo para mostrar resultados en la consola
    private void UpdateCounts()
    {
        // Mostrar resultados en la consola
        Debug.Log("Resultados del Test de Toulouse:");
        Debug.Log("Correctos seleccionados (Variable A): " + correctSelectedCount);
        Debug.Log("Incorrectos seleccionados (Variable E): " + incorrectSelectedCount);
        Debug.Log("Correctos no seleccionados (Variable O): " + correctNotSelectedCount);
        Debug.Log("Total seleccionados (Variable R): " + totalSelectedCount);
        Debug.Log("---------------------------------");

    }

    // M�todo para calcular y mostrar IGAP e ICI
    private void CalculateIndices()
    {
        // Calcular IGAP
        int IGAP = correctSelectedCount - (incorrectSelectedCount + correctNotSelectedCount);
        // Calcular ICI
        double ICI = (totalSelectedCount > 0)
            ? ((correctSelectedCount - incorrectSelectedCount) / (double)totalSelectedCount) * 100
            : 0;

        // Calcular m�tricas relevantes
        int totalFallas = incorrectSelectedCount + correctNotSelectedCount;
        double porcentajeFallas = (correctSelectedCount > 0)
            ? (double)totalFallas / correctSelectedCount * 100
            : 0;
        double limiteErrores = correctNotSelectedCount * 2 / 5.0;

        // Mostrar resultados en la consola
        Debug.Log($"�ndice Global de Atenci�n y Percepci�n (IGAP): {IGAP}");
        Debug.Log($"�ndice de Control de la Impulsividad (ICI): {ICI:F2}%");
        Debug.Log($"Porcentaje de Fallas: {porcentajeFallas:F2}%");
        Debug.Log("---------------------------------");

        // Crear mensaje de resultados para mostrar al usuario
        string resultsMessage = $"Resultados del Test de Toulouse:\nIGAP: {IGAP}\nICI: {ICI:F2}%";
        ShowNotification(resultsMessage);

        // Evaluaci�n cuantitativa y cualitativa
        string evaluacion = EvaluateTestResults(correctSelectedCount, incorrectSelectedCount, correctNotSelectedCount, porcentajeFallas, limiteErrores);

        // Mostrar evaluaci�n en consola para depuraci�n
        Debug.Log(evaluacion);

        // Guardar los resultados en Firestore
        SaveResultsToFirestore(IGAP, ICI, evaluacion);
    }

    // M�todo para evaluar resultados y generar una evaluaci�n cualitativa
    private string EvaluateTestResults(int correctSelectedCount, int incorrectSelectedCount, int correctNotSelectedCount, double porcentajeFallas, double limiteErrores)
    {
        string evaluacion = "";


        // Secci�n de Aciertos
        string evaluacionAciertos = "Evaluaci�n de Aciertos:\n";
        if (correctSelectedCount < 80)
        {
            evaluacionAciertos += "- La persona puede presentar una inhibici�n an�mica.\n";
        }
        else if (correctSelectedCount < 100)
        {
            evaluacionAciertos += "- El n�mero de aciertos es inferior a 100, lo que puede denotar una inhibici�n an�mica.\n";
        }

        // Secci�n de Errores
        string evaluacionErrores = "Evaluaci�n de Errores:\n";
        if (incorrectSelectedCount > limiteErrores)
        {
            evaluacionErrores += "- Los errores superan las dos quintas partes de las omisiones. Esto puede indicar falta de atenci�n.\n";
        }
        if (incorrectSelectedCount > correctNotSelectedCount)
        {
            evaluacionErrores += "- Los errores superan las omisiones, lo que podr�a interpretarse como falta de inteligencia. Esto debe corroborarse con otros tests.\n";
        }

        // Secci�n de Fallas
        string evaluacionFallas = "Evaluaci�n de Fallas:\n";
        if (porcentajeFallas > 10)
        {
            if (porcentajeFallas > 20)
            {
                evaluacionFallas += "- El n�mero de fallas supera el 20% de los aciertos, indicando una falla aguda en la concentraci�n.\n";
            }
            else
            {
                evaluacionFallas += "- El n�mero de fallas supera el 10% de los aciertos, lo que indica problemas de concentraci�n.\n";
            }
        }

        // Secci�n de Omisiones
        string evaluacionOmisiones = "Evaluaci�n de Omisiones:\n";
        if (correctNotSelectedCount > 0)
        {
            evaluacionOmisiones += $"- Hubo {correctNotSelectedCount} omisiones que pueden afectar la evaluaci�n general.\n";
        }

        // Concatenar todas las evaluaciones
        evaluacion = evaluacionAciertos + "\n" + evaluacionErrores + "\n" + evaluacionFallas + "\n" + evaluacionOmisiones;

        // Evaluaci�n por defecto si no hay observaciones significativas
        if (string.IsNullOrWhiteSpace(evaluacion.Trim()))
        {
            evaluacion = "Los resultados no presentan indicadores significativos de problemas en la atenci�n o concentraci�n.\n";
        }

        return evaluacion;
    }


    // M�todo para guardar resultados en Firestore
    private void SaveResultsToFirestore(int IGAP, double ICI, string evaluacion)
    {
        string userId = auth.CurrentUser?.UserId; // Obt�n el ID del usuario que inici� sesi�n

        if (userId != null)
        {
            DocumentReference docRef = db.Collection("ResultadosTest").Document(userId);

            // Obtener los datos existentes para decidir si actualizar o crear nuevos campos
            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error al obtener los resultados existentes: " + task.Exception);
                    return;
                }

                var snapshot = task.Result;
                bool actualizarResultados = snapshot.Exists && snapshot.ContainsField("IGAP");

                // Preparar los datos para guardar en Firestore
                var results = new Dictionary<string, object>
            {
                { "user_id", userId },
                { "timestamp", FieldValue.ServerTimestamp },
                { "time_remaining", timeRemaining }
            };

                if (actualizarResultados)
                {
                    // Si ya hay resultados, mover los existentes a los campos "actualizado"
                    results["IGAP_actualizado"] = IGAP;
                    results["ICI_actualizado"] = ICI;
                    results["evaluacion_actualizada"] = evaluacion;

                    // Mantener los valores iniciales intactos
                    results["IGAP"] = snapshot.GetValue<int>("IGAP");
                    results["ICI"] = snapshot.GetValue<double>("ICI");
                    results["evaluacion"] = snapshot.GetValue<string>("evaluacion");

                }
                else
                {
                    // Guardar como valores iniciales si es la primera vez
                    results["IGAP"] = IGAP;
                    results["ICI"] = ICI;
                    results["evaluacion"] = evaluacion;
                }

                // Guardar los resultados en Firestore
                docRef.SetAsync(results, SetOptions.MergeAll).ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        Debug.Log("Resultados guardados en Firestore con �xito.");
                    }
                    else
                    {
                        Debug.LogError("Error al guardar los resultados: " + task.Exception);
                    }
                });
            });
        }
        else
        {
            Debug.LogError("No se pudo obtener el ID del usuario.");
        }


    }

    // M�todo para reiniciar el temporizador
    private void ResetTimer()
    {
        timeRemaining = totalTime; // Reiniciar el tiempo
        UpdateTimerText(); // Actualizar el texto del temporizador al iniciar
        InvokeRepeating(nameof(UpdateTimer), 1f, 1f); // Llamar a UpdateTimer cada segundo
    }

    // M�todo para actualizar el temporizador
    private void UpdateTimer()
    {
        if (timeRemaining > 0)
        {
            timeRemaining--; // Decrementa el tiempo restante
            UpdateTimerText(); // Actualiza el texto del temporizador
        }
        else
        {
            CancelInvoke(nameof(UpdateTimer)); // Detener la invocaci�n cuando el tiempo se agote
            NotifyTimeUp(); // Notificar que el tiempo ha terminado
        }
    }

    // M�todo para actualizar el texto del temporizador
    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60); // Obtiene los minutos
        int seconds = Mathf.FloorToInt(timeRemaining % 60); // Obtiene los segundos
        timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds); // Formato MM:SS
    }

    // M�todo para notificar que el tiempo ha terminado
    private void NotifyTimeUp()
    {
        Debug.Log("El tiempo se ha agotado. Debes enviar los resultados.");
        SetSymbolButtonsInteractable(false); // Desactivar los botones de s�mbolo

        // Mostrar una notificaci�n indicando que el tiempo se ha agotado
        ShowNotification("El tiempo se ha agotado, por favor presiona el bot�n enviar.", "Tiempo agotado");
    }

    // M�todo para activar o desactivar botones de s�mbolos
    private void SetSymbolButtonsInteractable(bool interactable)
    {
        foreach (Button button in symbolButtons)
        {
            button.interactable = interactable; // Activar o desactivar los botones
        }
    }

    // M�todo para mostrar el panel de notificaci�n
    private void ShowNotification(string message, string title = "Resultados")
    {
        notificationTitle.text = title; // Establecer el t�tulo
        notificationMessage.text = message; // Establecer el mensaje
        notificationPanel.SetActive(true); // Mostrar el panel de notificaci�n
    }

    // M�todo para ocultar el panel de notificaci�n
    public void HideNotification()
    {
        notificationPanel.SetActive(false); // Ocultar el panel de notificaci�n
        Debug.Log("Cambiando a la escena de Test");
        SceneManager.LoadScene("Test");
    }
}
