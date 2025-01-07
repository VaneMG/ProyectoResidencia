using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EstadosNiveles : MonoBehaviour
{
    public bool IsPurchased = false; // Se activar� cuando se desbloquee el nivel
    private Button button; // Referencia al componente Button

    // Colores personalizados
    private Color colorDesactivado = new Color32(161, 130, 98, 255); // a18262
    private Color colorActivado = new Color32(188, 156, 123, 255);   // bc9c7b

    void Start()
    {
        button = GetComponent<Button>(); // Obtener el componente Button
        if (button == null)
        {
            Debug.LogError("El componente Button no est� adjunto a este GameObject.");
            return;
        }

        ActualizarEstadoVisual();
    }

    public void ActualizarEstadoVisual()
    {
        if (IsPurchased)
        {
            button.interactable = false; // Desactiva la interacci�n del bot�n
            CambiarColor(colorDesactivado);
        }
        else
        {
            button.interactable = true; // Activa la interacci�n del bot�n
            CambiarColor(colorActivado);
        }
    }

    private void CambiarColor(Color color)
    {
        ColorBlock colorBlock = button.colors;
        colorBlock.normalColor = color; // Color del bot�n en estado normal
        colorBlock.highlightedColor = color; // Color cuando el bot�n est� resaltado
        colorBlock.pressedColor = color; // Color cuando el bot�n est� presionado
        colorBlock.selectedColor = color; // Color cuando el bot�n est� seleccionado
        colorBlock.disabledColor = color; // Color cuando el bot�n est� deshabilitado
        button.colors = colorBlock;
    }
}
