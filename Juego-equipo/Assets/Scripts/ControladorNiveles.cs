using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;


public class ControladorNiveles : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseFirestore db;

    public static ControladorNiveles instancia;
    public Button[] botonesNiveles;
    public int desbloquearNiveles;

    private string userId;

    public void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
        }
    }

    void Start()
    {
        // Inicializar Firebase
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        if (auth.CurrentUser != null)
        {
            userId = auth.CurrentUser.UserId;
            CargarNivelesDesbloqueados();
        }
        else
        {
            Debug.LogError("Usuario no autenticado. Inicie sesión antes de acceder a los niveles.");
        }
    }

    private void CargarNivelesDesbloqueados()
    {
        db.Collection("Niveles")
          .Document(userId)
          .GetSnapshotAsync()
          .ContinueWithOnMainThread(task =>
          {
              if (task.IsCompleted && task.Result.Exists)
              {
                  Dictionary<string, object> data = task.Result.ToDictionary();
                  int nivelesDesbloqueados = data.ContainsKey("nivelesDesbloqueados")
                      ? int.Parse(data["nivelesDesbloqueados"].ToString())
                      : 1;

                  for (int i = 0; i < botonesNiveles.Length; i++)
                  {
                      botonesNiveles[i].interactable = i < nivelesDesbloqueados;
                  }
              }
              else
              {
                  Debug.Log("No se encontraron niveles desbloqueados. Configurando valores predeterminados.");
                  for (int i = 0; i < botonesNiveles.Length; i++)
                  {
                      botonesNiveles[i].interactable = i == 0; // Solo el primer nivel desbloqueado
                  }
              }
          });
    }

    public void AumentarNiveles()
    {
        if (desbloquearNiveles > 0 && auth.CurrentUser != null)
        {
            db.Collection("Niveles")
              .Document(userId)
              .SetAsync(new Dictionary<string, object>
              {
                  { "nivelesDesbloqueados", desbloquearNiveles },
                  { "ultimaActualizacion", FieldValue.ServerTimestamp }
              }, SetOptions.MergeFields(new[] { "nivelesDesbloqueados", "ultimaActualizacion" }))
              .ContinueWithOnMainThread(task =>
              {
                  if (task.IsCompleted)
                  {
                      Debug.Log($"Progreso guardado: {desbloquearNiveles} niveles desbloqueados.");
                  }
                  else
                  {
                      Debug.LogError("Error al guardar niveles desbloqueados.");
                  }
              });
        }
    }
}

