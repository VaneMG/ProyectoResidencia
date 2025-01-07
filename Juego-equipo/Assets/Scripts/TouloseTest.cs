using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Para usar TextMeshPro
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using Firebase.Firestore; // Firestore para guardar datos
using Firebase.Auth; // Firebase Auth para la autenticación

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
    public GameObject notificationPanel; // El panel de notificación
    public TextMeshProUGUI notificationTitle; // El título en el panel
    public TextMeshProUGUI notificationMessage; // El mensaje en el panel

    // Variables para el temporizador
    private float totalTime = 600f; // 10 minutos en segundos
    private float timeRemaining;
    public TMP_Text timerText; // Referencia al TextMeshPro para mostrar el temporizador
    public Button[] symbolButtons; // Referencia a los botones de símbolo
    public Button enviarButton; // Referencia al botón "Enviar"
    public Button closeButton; // Referencia al botón de cerrar

    // Referencia a Firestore y Firebase Auth
    private FirebaseFirestore db;
    private FirebaseAuth auth;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance; // Inicializa Firestore
        auth = FirebaseAuth.DefaultInstance; // Inicializa Firebase Auth
        ResetTimer(); // Inicializar el temporizador al comenzar
        SetSymbolButtonsInteractable(true); // Habilitar botones de símbolos al inicio

        // Asignar eventos de clic a cada botón de símbolo
        foreach (Button button in symbolButtons)
        {
            string buttonName = button.name; // Obtén el nombre del botón
            button.onClick.AddListener(() => OnSymbolButtonClick(buttonName, button));
        }

        // Asignar evento de clic al botón de cerrar
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    // Método para manejar el clic del botón de cerrar
    public void OnCloseButtonClick()
    {
        Debug.Log("Cerrando y regresando al menú principal...");
        SceneManager.LoadScene("MenuPrincipal 1"); // Cambiar "MenuPrincipal" por el nombre real de tu escena
    }

    // Método para ser llamado al hacer clic en un botón de símbolo
    public void OnSymbolButtonClick(string buttonName, Button button)
    {
        if (timeRemaining <= 0)
        {
            Debug.Log("El tiempo se ha agotado. Ya no puedes seleccionar más símbolos.");
            return; // Salir si el tiempo se ha agotado
        }

        if (selectedButtons.Contains(buttonName))
        {
            // Deseleccionar el botón
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
                Debug.Log("Ya has seleccionado 3 símbolos. No puedes seleccionar más.");
                return; // Limitar a 3 selecciones
            }
            // Agregar el botón a la lista de seleccionados
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

            button.image.color = Color.green; // Cambiar el color del botón para indicar selección
        }
    }

    // Método para ser llamado al hacer clic en el botón "Enviar"
    public void OnEnviarButtonClick()
    {
        // Detener el temporizador al hacer clic en enviar
        CancelInvoke(nameof(UpdateTimer));

        // Desactivar los botones de símbolos para evitar más selecciones
        SetSymbolButtonsInteractable(false);

        Debug.Log("Enviando resultados...");
        CalculateNotSelected(); // Calcular los correctos no seleccionados
        UpdateCounts(); // Mostrar los resultados
        CalculateIndices(); // Calcular IGAP e ICI
    }

    // Método para calcular los correctos no seleccionados
    private void CalculateNotSelected()
    {
        correctNotSelectedCount = correctButtons.Count - correctSelectedCount; // Variable O
    }

    // Método para mostrar resultados en la consola
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

    // Método para calcular y mostrar IGAP e ICI
    private void CalculateIndices()
    {
        // Calcular IGAP
        int IGAP = correctSelectedCount - (incorrectSelectedCount + correctNotSelectedCount);
        // Calcular ICI
        double ICI = (totalSelectedCount > 0)
            ? ((correctSelectedCount - incorrectSelectedCount) / (double)totalSelectedCount) * 100
            : 0;

        // Calcular métricas relevantes
        int totalFallas = incorrectSelectedCount + correctNotSelectedCount;
        double porcentajeFallas = (correctSelectedCount > 0)
            ? (double)totalFallas / correctSelectedCount * 100
            : 0;
        double limiteErrores = correctNotSelectedCount * 2 / 5.0;

        // Mostrar resultados en la consola
        Debug.Log($"Índice Global de Atención y Percepción (IGAP): {IGAP}");
        Debug.Log($"Índice de Control de la Impulsividad (ICI): {ICI:F2}%");
        Debug.Log($"Porcentaje de Fallas: {porcentajeFallas:F2}%");
        Debug.Log("---------------------------------");

        // Crear mensaje de resultados para mostrar al usuario
        string resultsMessage = $"Resultados del Test de Toulouse:\nIGAP: {IGAP}\nICI: {ICI:F2}%";
        ShowNotification(resultsMessage);

        // Evaluación cuantitativa y cualitativa
        string evaluacion = EvaluateTestResults(correctSelectedCount, incorrectSelectedCount, correctNotSelectedCount, porcentajeFallas, limiteErrores);

        // Mostrar evaluación en consola para depuración
        Debug.Log(evaluacion);

        // Guardar los resultados en Firestore
        SaveResultsToFirestore(IGAP, ICI, evaluacion);
    }

    // Método para evaluar resultados y generar una evaluación cualitativa
    private string EvaluateTestResults(int correctSelectedCount, int incorrectSelectedCount, int correctNotSelectedCount, double porcentajeFallas, double limiteErrores)
    {
        string evaluacion = "";


        // Sección de Aciertos
        string evaluacionAciertos = "Evaluación de Aciertos:\n";
        if (correctSelectedCount < 80)
        {
            evaluacionAciertos += "- La persona puede presentar una inhibición anímica.\n";
        }
        else if (correctSelectedCount < 100)
        {
            evaluacionAciertos += "- El número de aciertos es inferior a 100, lo que puede denotar una inhibición anímica.\n";
        }

        // Sección de Errores
        string evaluacionErrores = "Evaluación de Errores:\n";
        if (incorrectSelectedCount > limiteErrores)
        {
            evaluacionErrores += "- Los errores superan las dos quintas partes de las omisiones. Esto puede indicar falta de atención.\n";
        }
        if (incorrectSelectedCount > correctNotSelectedCount)
        {
            evaluacionErrores += "- Los errores superan las omisiones, lo que podría interpretarse como falta de inteligencia. Esto debe corroborarse con otros tests.\n";
        }

        // Sección de Fallas
        string evaluacionFallas = "Evaluación de Fallas:\n";
        if (porcentajeFallas > 10)
        {
            if (porcentajeFallas > 20)
            {
                evaluacionFallas += "- El número de fallas supera el 20% de los aciertos, indicando una falla aguda en la concentración.\n";
            }
            else
            {
                evaluacionFallas += "- El número de fallas supera el 10% de los aciertos, lo que indica problemas de concentración.\n";
            }
        }

        // Sección de Omisiones
        string evaluacionOmisiones = "Evaluación de Omisiones:\n";
        if (correctNotSelectedCount > 0)
        {
            evaluacionOmisiones += $"- Hubo {correctNotSelectedCount} omisiones que pueden afectar la evaluación general.\n";
        }

        // Concatenar todas las evaluaciones
        evaluacion = evaluacionAciertos + "\n" + evaluacionErrores + "\n" + evaluacionFallas + "\n" + evaluacionOmisiones;

        // Evaluación por defecto si no hay observaciones significativas
        if (string.IsNullOrWhiteSpace(evaluacion.Trim()))
        {
            evaluacion = "Los resultados no presentan indicadores significativos de problemas en la atención o concentración.\n";
        }

        return evaluacion;
    }


    // Método para guardar resultados en Firestore
    private void SaveResultsToFirestore(int IGAP, double ICI, string evaluacion)
    {
        string userId = auth.CurrentUser?.UserId; // Obtén el ID del usuario que inició sesión

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
                        Debug.Log("Resultados guardados en Firestore con éxito.");
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

    // Método para reiniciar el temporizador
    private void ResetTimer()
    {
        timeRemaining = totalTime; // Reiniciar el tiempo
        UpdateTimerText(); // Actualizar el texto del temporizador al iniciar
        InvokeRepeating(nameof(UpdateTimer), 1f, 1f); // Llamar a UpdateTimer cada segundo
    }

    // Método para actualizar el temporizador
    private void UpdateTimer()
    {
        if (timeRemaining > 0)
        {
            timeRemaining--; // Decrementa el tiempo restante
            UpdateTimerText(); // Actualiza el texto del temporizador
        }
        else
        {
            CancelInvoke(nameof(UpdateTimer)); // Detener la invocación cuando el tiempo se agote
            NotifyTimeUp(); // Notificar que el tiempo ha terminado
        }
    }

    // Método para actualizar el texto del temporizador
    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60); // Obtiene los minutos
        int seconds = Mathf.FloorToInt(timeRemaining % 60); // Obtiene los segundos
        timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds); // Formato MM:SS
    }

    // Método para notificar que el tiempo ha terminado
    private void NotifyTimeUp()
    {
        Debug.Log("El tiempo se ha agotado. Debes enviar los resultados.");
        SetSymbolButtonsInteractable(false); // Desactivar los botones de símbolo

        // Mostrar una notificación indicando que el tiempo se ha agotado
        ShowNotification("El tiempo se ha agotado, por favor presiona el botón enviar.", "Tiempo agotado");
    }

    // Método para activar o desactivar botones de símbolos
    private void SetSymbolButtonsInteractable(bool interactable)
    {
        foreach (Button button in symbolButtons)
        {
            button.interactable = interactable; // Activar o desactivar los botones
        }
    }

    // Método para mostrar el panel de notificación
    private void ShowNotification(string message, string title = "Resultados")
    {
        notificationTitle.text = title; // Establecer el título
        notificationMessage.text = message; // Establecer el mensaje
        notificationPanel.SetActive(true); // Mostrar el panel de notificación
    }

    // Método para ocultar el panel de notificación
    public void HideNotification()
    {
        notificationPanel.SetActive(false); // Ocultar el panel de notificación
        Debug.Log("Cambiando a la escena de Test");
        SceneManager.LoadScene("Test");
    }
}
