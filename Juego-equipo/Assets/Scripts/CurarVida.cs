using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurarVida : MonoBehaviour
{
    public int curacion;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out vidasPlay VidaPLay))
        {
            VidaPLay.CurarVida(curacion);
            Destroy(gameObject); // Elimina el objeto después de otorgar la vida
        }
    }
}
