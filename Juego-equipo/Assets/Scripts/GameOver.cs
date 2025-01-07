using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverPanel;

    // Método para mostrar el panel de Game Over
    public void MostrarGameOver()
    {
        gameOverPanel.SetActive(true);  // Muestra el panel de Game Over
    }


    public void ReiniciarNivel()
    {
        // Resetea el puntaje antes de recargar la escena
        if (PuntosGuargar.Instance != null)
        {
            PuntosGuargar.Instance.ReiniciarPuntajeYMonedas();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // Boton para reiniciar el juego 
    }

    public void MenuPrincipal()
    {
        SceneManager.LoadScene("MenuPrincipal 1");
    }
}
