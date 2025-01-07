using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using TMPro; // Necesario para TextMeshPro
using Firebase.Auth; // Asegúrate de tener la librería de autenticación de Firebase
using Firebase.Firestore;
using Firebase.Extensions;
using System;

public class GameManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseFirestore db;
    public string nivelID; // Nivel específico que quieres mostrar

    [Header("ProgresoUsuario TextMesh")]
    public List<TMP_Text> progresoUsuarioTexts; // Lista de TextMesh para ProgresoUsuario (42)

    [Header("ResultadosTest TextMesh")]
    public List<TMP_Text> resultadosTestTexts; // Lista de TextMesh para ResultadosTest (3)

    [Header("ResultadosTestActuali TextMesh")]
    public List<TMP_Text> resultadosTestTextsActualizado; // Lista de TextMesh para ResultadosTest (3)

    void Start()
    {
        // Inicializamos Firebase
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        // Escuchar cambios en el documento ProgresoUsuario para el nivel específico
        ListenToProgresoUsuario();

        // Escuchar cambios en el documento ResultadosTest
        ListenToResultadosTest();

        // Escuchar cambios en el documento ResultadosTest
        ListenToResultadosTestActualizado();
    }

    private void ListenToProgresoUsuario()
    {
        // Obtener el ID del usuario actual
        string userID = auth.CurrentUser.UserId;
        db.Collection("ProgresoUsuario")
          .WhereEqualTo("user_id", userID)
          .WhereEqualTo("nivel_id", nivelID)
          .Listen(snapshot =>
          {
              if (snapshot != null && snapshot.Count > 0) // Verifica si el snapshot tiene documentos
              {
                  foreach (var doc in snapshot.Documents)
                  {
                      // Asigna los valores a los TextMesh de ProgresoUsuario
                      SetText(progresoUsuarioTexts[0], doc, "puntuacion_actual", "puntos");
                      SetText(progresoUsuarioTexts[1], doc, "tiempo_juego", "segundos");
                      SetText(progresoUsuarioTexts[2], doc, "vidas_jugador", "vidas");
                      SetText(progresoUsuarioTexts[3], doc, "puntuacion_actualizada", "puntos");
                      SetText(progresoUsuarioTexts[4], doc, "tiempo_actualizado", "segundos");
                      SetText(progresoUsuarioTexts[5], doc, "vidas_actualizadas", "vidas");

                  }
              }
              else
              {
                  ClearTexts(progresoUsuarioTexts); // Limpia los textos si no hay datos
              }
          });
    }

    private void ListenToResultadosTest()
    {
        // Obtener el ID del usuario actual
        string userID = auth.CurrentUser.UserId;

        db.Collection("ResultadosTest").Document(userID).Listen(snapshot =>
        {
            if (snapshot.Exists)
            {
                // Asigna los valores a los TextMesh de ResultadosTest
                SetText(resultadosTestTexts[0], snapshot, "ICI");
                SetText(resultadosTestTexts[1], snapshot, "IGAP");
                SetText(resultadosTestTexts[2], snapshot, "evaluacion");

            }
            else
            {
                ClearTexts(resultadosTestTexts); // Limpia los textos si no hay datos
            }
        });
    }

    private void ListenToResultadosTestActualizado()
    {
        // Obtener el ID del usuario actual
        string userID = auth.CurrentUser.UserId;

        db.Collection("ResultadosTest").Document(userID).Listen(snapshot =>
        {
            if (snapshot.Exists)
            {
                // Asigna los valores a los TextMesh de ResultadosTest
                SetText(resultadosTestTextsActualizado[0], snapshot, "ICI_actualizado");
                SetText(resultadosTestTextsActualizado[1], snapshot, "IGAP_actualizado");
                SetText(resultadosTestTextsActualizado[2], snapshot, "evaluacion_actualizada");

            }
            else
            {
                ClearTexts(resultadosTestTextsActualizado); // Limpia los textos si no hay datos
            }
        });
    }


    private void SetText(TMP_Text textMesh, DocumentSnapshot doc, string field, string suffix = "")
    {
        if (textMesh != null && doc.TryGetValue(field, out object value))
        {
            textMesh.text = $"{value} {suffix}"; // Agrega el texto adicional (suffix)
        }
    }

    private void ClearTexts(List<TMP_Text> texts)
    {
        foreach (var textMesh in texts)
        {
            if (textMesh != null)
            {
                textMesh.text = "";
            }
        }
    }
}
