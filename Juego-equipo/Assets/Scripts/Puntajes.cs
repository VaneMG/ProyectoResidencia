using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Puntajes : MonoBehaviour
{
    private TextMeshProUGUI textMeshProUGUI;

    private void Start()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        PuntosGuargar.Instance.sumarPuntosEvent += CambiarTexto;
    }


    public void CambiarTexto(object sender, PuntosGuargar.SumarPuntosEventArgs e)
    {
        textMeshProUGUI.text = e.puntajeActualEvent.ToString();
    }

}
