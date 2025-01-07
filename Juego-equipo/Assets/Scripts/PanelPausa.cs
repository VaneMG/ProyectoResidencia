using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelPausa : MonoBehaviour
{
    public GameObject panelJuego;
    public GameObject tiempoRelog;
    public GameObject canvasVida;
    public GameObject seccionMenu;
    public GameObject canva;
    public GameObject visualizarPuntaje;

    // Método que muestra el panel de pausa y pausa el juego
    public void MostrarPanelJuego()
    {
        seccionMenu.SetActive(false); // Oculta el panel SeccionMenu
        panelJuego.SetActive(true); // Muestra el panel de pausa PanelJuego
        Time.timeScale = 0f; // Pausa el tiempo del juego
    }

    // Método para ocultar el panel de pausa y reanudar el juego
    public void OcultarPanelJuego()
    {
        panelJuego.SetActive(false); // Oculta el panel de pausa
        seccionMenu.SetActive(true); // Muestra de nuevo el panel SeccionMenu
        Time.timeScale = 1f; // Restaura el tiempo del juego
    }

    // Método para salir al menú principal
    public void IrAlMenuPrincipal()
    {
        //Time.timeScale = 1f; // Asegúrate de restaurar el tiempo antes de cambiar de escena
        SceneManager.LoadScene("MenuPrincipal 1");
    }

    void ocultarComponentes()
    {
        tiempoRelog.SetActive(false);
        canvasVida.SetActive(false);
        canva.SetActive(false);
        visualizarPuntaje.SetActive(false);
    }
}
