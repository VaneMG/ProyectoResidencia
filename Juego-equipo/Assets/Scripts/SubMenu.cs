using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubMenu : MonoBehaviour
{
    // 
    public void Cambiar1()
    {
        SceneManager.LoadScene("Submenu");
    }


    public void Cambiar2()
    {
        SceneManager.LoadScene("PerfilUsuario");
    }

    //
    public void Cambiar3()
    {
        SceneManager.LoadScene("MenuPrincipal 1");
    }

    public void Cambiar5()
    {
        SceneManager.LoadScene("Avatar");
    }

    public void Cambiar4()
    {
        SceneManager.LoadScene("SeleccionPersonaje");
    }

    public void Cambiar6()
    {
        SceneManager.LoadScene("TestInformación");
    }

    // Método para cambiar a Nivel 4
    public void CambiarANivel4()
    {
        SceneManager.LoadScene("Nivel 4");
    }

    public void CambiarNivel1()
    {
        SceneManager.LoadScene("Juego");
    }

    // 
    public void CambiarNivel2()
    {
        
        SceneManager.LoadScene("Nivel 2");
    }

    //
    public void CambiarNivel3()
    {
        
        SceneManager.LoadScene("Nivel 3");
    }

    // Método para cambiar a Nivel 5
    public void CambiarANivel5()
    {
        SceneManager.LoadScene("Nivel 5");
    }

    // Método para cambiar a Nivel 6
    public void CambiarANivel6()
    {
        
        SceneManager.LoadScene("Nivel 6");
    }

    // Método para cambiar a Nivel 7
    public void CambiarANivel7()
    {
        SceneManager.LoadScene("Nivel 7");
    } 
}
