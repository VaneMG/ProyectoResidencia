using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class vidasPlay : MonoBehaviour
{
    //Creacion de variables

    public int vidaActual;
    public int vidaMaxima;

    public UnityEvent<int> cambioVida;  //crea un evento para el juego

    public GameOver gameOverManager; // Referencia al script GameOver

    // Start is called before the first frame update
    void Start()
    {
        vidaActual = vidaMaxima;
        cambioVida.Invoke(vidaActual);
    }


    // Hacer un metodo para tomar el daño del jugador
    public void TomarDaño(int cantidadDaño)
    {
        int vidaTemporal = vidaActual - cantidadDaño;

        if (vidaTemporal < 0)
        {
            vidaActual = 0;
        }
        else
        {
            vidaActual = vidaTemporal;
        }

        cambioVida.Invoke(vidaActual); 

        if (vidaActual <= 0)
        {
            if (gameOverManager != null)  // Asegúrate de que el gameOverManager está asignado
            {
                gameOverManager.MostrarGameOver();  // Llamar al método para activar el panel de Game Over
                                                    // Detener el reloj cuando el jugador muere
                FindObjectOfType<Reloj>().DetenerReloj();  // Asegúrate de tener solo un objeto RelojJuego en la escena
            }
            else
            {
                Debug.LogError("gameOverManager no está asignado en vidasPlay.");
            }

            Destroy(gameObject, 2.0f); // Destruye al jugador después de 2 segundos, por ejemplo

        }

    }

    public void CurarVida(int cantidadCuracion)
    {
        int vidaTemporal = vidaActual + cantidadCuracion;

        if(vidaTemporal > vidaMaxima)
        {
            vidaActual = vidaMaxima;
        }
        else
        {
            vidaActual = vidaTemporal;
        }

        cambioVida.Invoke(vidaActual);
    }

}
