using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

// Este JSON aqui se guardan los datos de los jugadores como vida, monedas y posicion en el juego.
public class DatosJuego
{
    //public Vector3 posicion;
    public Dictionary<string, int> puntajesPorNivel = new Dictionary<string, int>();
    public int vidaActual; // Nueva variable para la vida actual
    public int vidaMaxima; // Nueva variable para la vida máxima
    public float tiempoDeJuego; // Nuevo campo para almacenar el tiempo de juego
    public string uid;
    public string personajeSeleccionado;  // Nuevo campo para el personaje seleccionado
    // Variables de monedas
    public Dictionary<string, int> monedasPorNivel = new Dictionary<string, int>();
}
