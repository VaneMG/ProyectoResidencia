using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetoDaño : MonoBehaviour
{
    public int daño;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))  // Asegúrate de que el objeto colisionado es el jugador
        {
            if (collision.TryGetComponent(out vidasPlay VidasPlay))
            {
                VidasPlay.TomarDaño(daño);
            }
            else
            {
                Debug.LogWarning("El jugador no tiene el componente vidasPlay asignado.");
            }
        }
        else
        {
            Debug.Log("Colisión con un objeto que no es el jugador: " + collision.name);
        }
    }
}
