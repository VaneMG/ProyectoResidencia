using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Reloj : MonoBehaviour
{
    public TextMeshProUGUI relojText;  // Referencia al TextMeshPro donde mostrarás el tiempo
    private float tiempoTranscurrido;
    private bool jugando = true;  // Esta variable controlará si el reloj sigue corriendo o no
    public float tiempoFinal; // Guarda el tiempo cuando el jugador gana

    private void Awake()
    {
        AsignarRelojText(); // Encuentra el TextMeshPro en la escena
        ReiniciarReloj();  // Inicializa el reloj

    }

    public void AsignarRelojText()
    {
        if (relojText == null)
        {
            relojText = FindObjectOfType<TextMeshProUGUI>();
            if (relojText == null)
            {
                Debug.LogError("No se encontró un TextMeshProUGUI en la escena.");
            }
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        tiempoTranscurrido = 0f;
        ActualizarReloj(0);  // Inicializa el reloj en 00:00
    }

    // Update is called once per frame
    void Update()
    {
       if (relojText == null)
        {
           AsignarRelojText();
        }

        if (jugando)
        {
            tiempoTranscurrido += Time.deltaTime;  // Aumenta el tiempo transcurrido
            ActualizarReloj(tiempoTranscurrido);   // Actualiza el reloj en pantalla
        }
    }


    void ActualizarReloj(float tiempo)
    {
        if (relojText == null)
        {
            Debug.LogWarning("El objeto relojText no está asignado.");
            return; // Evita ejecutar el código si relojText es null
        }

        int minutos = Mathf.FloorToInt(tiempo / 60F);
        int segundos = Mathf.FloorToInt(tiempo % 60F);
        relojText.text = minutos.ToString("00") + ":" + segundos.ToString("00");  // Formato 00:00
    }

    // Método para detener el reloj cuando el jugador muere
    public void DetenerReloj()
    {
        jugando = false;
    }
    public void ReiniciarReloj()
    {
        
        tiempoTranscurrido = 0f;
        jugando = true;
        ActualizarReloj(0);  // Reinicia la visualización del reloj en 00:00
    }

    // Método para detener el reloj cuando el jugador gana (aun no implementado)
    public void GanarJuego()
    {
        jugando = false;
        tiempoFinal = tiempoTranscurrido; // Guarda el tiempo actual cuando el jugador gana


            // Formatea el tiempo final como 00:00
            int minutos = Mathf.FloorToInt(tiempoFinal / 60F);
            int segundos = Mathf.FloorToInt(tiempoFinal % 60F);
            string tiempoFormateado = minutos.ToString("00") + ":" + segundos.ToString("00");

            Debug.Log("Tiempo final del jugador: " + tiempoFormateado); // Mostrar en consola
       
    }

    public float GetTiempoTranscurrido()
    {
        return tiempoTranscurrido;
    }

}

