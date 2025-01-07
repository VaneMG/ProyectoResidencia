using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

//using System.Linq;

public class CorazonesUI : MonoBehaviour
{

    // Crear variables para el diseño de los corazones
    public List<Image> listaCorazones;
    public GameObject corazonPrefab;
    public vidasPlay VidasPlay;  // que l script lo tiene el gameobject jugador (pero este codigo de corazonesUni lo tiene un objecto que pide este componente)
    public int indexActual;
    public Sprite corazonLleno;
    public Sprite corazonVacio;
    public UnityEvent<int> OnVidaCambiada = new UnityEvent<int>();


    private void Awake()
    {
        VidasPlay.cambioVida.AddListener(CambiarCorazones);
        OnVidaCambiada.AddListener(CambiarCorazones);
    }

    public void RecibirCambioDeVida(int vidaActual)
    {
        OnVidaCambiada.Invoke(vidaActual);
    }

    private void CambiarCorazones(int vidaActual)
    {
        if (!listaCorazones.Any())
        {
            CrearCorazones(vidaActual);
        }
        else
        {
            CambiarVida(vidaActual);
        }
    }

    private void CrearCorazones(int cantidadMaximaVida)
    {
        for (int i = 0; i < cantidadMaximaVida; i++)
        {
            GameObject corazon = Instantiate(corazonPrefab, transform);

            listaCorazones.Add(corazon.GetComponent<Image>());
        }

        indexActual = cantidadMaximaVida - 1;


    }

    private void CambiarVida(int vidaActual)
    {
        if (vidaActual <= indexActual)
        {
            QuitarCorazones(vidaActual);
        }
        else
        {
            AgregarCorazones(vidaActual);
        }
    }

    private void QuitarCorazones(int vidaActual)
    {
        for (int i = indexActual; i >= vidaActual; i--)
        {
            indexActual = i;
            listaCorazones[indexActual].sprite = corazonVacio;
        }
    }

    private void AgregarCorazones(int vidaActual)
    {
        for (int i = indexActual; i < vidaActual; i++)
        {
            indexActual = i;
            listaCorazones[indexActual].sprite = corazonLleno;
        }
    }

}
