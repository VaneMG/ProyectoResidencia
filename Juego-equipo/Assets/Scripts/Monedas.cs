using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monedas : MonoBehaviour
{
    // Crear variables

    [SerializeField] private int cantidadPuntos;
    [SerializeField] private int idmoneda;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (PuntosGuargar.Instance != null)
            {
                PuntosGuargar.Instance.SumarPuntos(cantidadPuntos);
                PuntosGuargar.Instance.SumaMonedas(idmoneda);
                Destroy(gameObject);  // Destruir la moneda
            }
            else
            {
                Debug.LogError("PuntosGuargar Instance es nulo");
            }
        }
    }

}
