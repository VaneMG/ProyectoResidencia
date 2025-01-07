using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeguimientoCamara : MonoBehaviour
{
    // Creando variables
    public Vector2 minCampos, maxCampos;
    public GameObject seguir;

    private bool objetoDestruido = false; // Bandera para evitar múltiples mensajes

    void Update()
    {
        if (seguir != null) // Verifica si el objeto 'seguir' aún existe
        {
            // Actualiza la posición de la cámara
            float posX = seguir.transform.position.x;
            float posY = seguir.transform.position.y;

            transform.position = new Vector3(
                Mathf.Clamp(posX, minCampos.x, maxCampos.x),
                Mathf.Clamp(posY, minCampos.y, maxCampos.y),
                transform.position.z);
        }
        else if (!objetoDestruido) // Solo muestra el mensaje una vez
        {
            Debug.LogWarning("El objeto 'seguir' ha sido destruido. La cámara ha dejado de seguir al jugador.");
            objetoDestruido = true; // Marca como destruido para evitar repetición
        }
    }
}


