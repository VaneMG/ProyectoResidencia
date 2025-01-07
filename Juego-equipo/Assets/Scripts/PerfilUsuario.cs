using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // Importa TextMeshPro
using Firebase;
using Firebase.Auth;
using UnityEngine.SceneManagement; // Importa para cambiar de escena

public class ProfilePageController : MonoBehaviour
{
    // Referencias a los textos del perfil
    public TMP_Text profileUserName_Text;
    public TMP_Text profileEmail_Text;

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;


    // Cambio de escena
    public void cambiarEscena(int numeroescena)
    {
        SceneManager.LoadScene(numeroescena);
    }

    private void Start()
    {
        // Inicializa Firebase Auth
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;

        if (user != null)
        {
            // Muestra el nombre de usuario y el correo
            profileUserName_Text.text = user.DisplayName;
            profileEmail_Text.text = user.Email;
        }
        else
        {
            Debug.LogError("No user is logged in");
        }
    }

    // Función para cerrar sesión
    public void LogOut()
    {
        auth.SignOut();  // Cierra la sesión del usuario en Firebase
        SceneManager.LoadScene(0);  // Cambia de escena al Login (suponiendo que el login es la escena 0)

        // Opcional: Limpiar los textos del perfil
        profileUserName_Text.text = "";
        profileEmail_Text.text = "";

        Debug.Log("User has been logged out.");
    }
}
