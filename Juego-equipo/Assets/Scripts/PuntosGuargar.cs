using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PuntosGuargar : MonoBehaviour
{
    // variables creadaS
    public static PuntosGuargar Instance;

    [SerializeField] private int puntajeActual;
    [SerializeField] private int puntajeMaximo;
    [SerializeField] private int totalMonedasActual;
    [SerializeField] private int totalMonedasMaximo;

    public event EventHandler<SumarPuntosEventArgs> sumarPuntosEvent;
    public event EventHandler<TotalMonedasEvenArgs> totalMonedasEvent;
    public class SumarPuntosEventArgs : EventArgs
    {
        public int puntajeActualEvent;
    }

    public class TotalMonedasEvenArgs : EventArgs
    {
        public int totalMonedasActualEvent;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // Destruye cualquier instancia duplicada
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Para que no se destruya al cambiar de escena
        }
    }

    public void SumarPuntos(int puntos)
    {
        puntajeActual += puntos;
        if (puntajeActual > puntajeMaximo)
        {
            puntajeMaximo = puntajeActual;
        }

        // Invocar el evento con los datos actualizados
        sumarPuntosEvent?.Invoke(this, new SumarPuntosEventArgs
        {
            puntajeActualEvent = puntajeActual,
        });

    }

    public void SumaMonedas(int monedas)
    {
        // Sumar monedas al total actual
        totalMonedasActual += monedas;
        if (totalMonedasActual > totalMonedasMaximo)
        {
            totalMonedasMaximo = totalMonedasActual;
        }

        totalMonedasEvent?.Invoke(this, new TotalMonedasEvenArgs
        {

            totalMonedasActualEvent = totalMonedasActual
        });
    }

    // Método para obtener el puntaje actual
    public int GetPuntaje()
    {
        return puntajeActual;
    }

    // Método para obtener las monedas actuales
    public int GetTotalMonedas()
    {
        return totalMonedasActual;
    }

    // Método para establecer el puntaje actual (para cuando se carguen los datos)
    public void SetPuntaje(int valor)
    {
        puntajeActual = valor;
    }

    // Método para establecer las monedas actuales (por ejemplo, al cargar datos)
    public void SetTotalMonedas(int valor)
    {
        totalMonedasActual = valor;
    }
    // Esto es cuando se reinicia el juego, los puntos y monedas se dejan en 0
    public void ReiniciarPuntajeYMonedas()
    {
        puntajeActual = 0;
        totalMonedasActual = 0;
    }


}
