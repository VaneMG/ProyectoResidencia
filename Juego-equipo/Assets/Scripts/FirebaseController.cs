using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // Importa TextMeshPro
using UnityEngine.UI;  // Aseg�rate de importar UnityEngine.UI para Toggle
using Firebase;
using Firebase.Auth;
using System;
using System.Threading.Tasks;
using Firebase.Extensions;
using UnityEngine.SceneManagement; // Libreria para el cambio de escena

public class FirebaseController : MonoBehaviour
{
    // Paneles de la UI
    public GameObject loginPanel, signupPanel, forgetPasswordPanel, notificationPanel;

    // Cambia InputField a TMP_InputField
    public TMP_InputField loginEmail, loginPassword, signupEmail, signupPassword, signupCPassword, signupUserName, forgetPassEmail;

    // Cambia Text a TMP_Text
    public TMP_Text notif_Title_Text, notif_Message_Text;

    // Toggle de "Recordarme"
    public Toggle rememberMe;

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    bool isSignIn = false;

    // Cambio de escena
    /* public void cambiarEscena(int numeroescena)
    {
        SceneManager.LoadScene(numeroescena);
    } */

    private void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                InitializeFirebase();
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }


    // Abrir el panel de inicio de sesi�n
    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        signupPanel.SetActive(false);
        forgetPasswordPanel.SetActive(false);
    }

    // Abrir el panel de registro
    public void OpenSignUpPanel()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);
        forgetPasswordPanel.SetActive(false);
    }

    // Abrir el panel de olvid� contrase�a
    public void OpenForgetPass()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        forgetPasswordPanel.SetActive(true);
    }

    // L�gica de inicio de sesi�n
    public void LoginUser()
    {
        if (string.IsNullOrEmpty(loginEmail.text) || string.IsNullOrEmpty(loginPassword.text))
        {
            notificationPanel.SetActive(false);
            return;
        }

        SignInUser(loginEmail.text,loginPassword.text);
    }

    // L�gica de registro de usuario
    public void SignUpUser()
    {
        if (string.IsNullOrEmpty(signupEmail.text) || string.IsNullOrEmpty(signupPassword.text) || string.IsNullOrEmpty(signupCPassword.text) || string.IsNullOrEmpty(signupUserName.text))
        {
            ShowNotificationMessage("Error", "Please fill in all the fields.");
            return;
        }
        CreateUser(signupEmail.text, signupPassword.text, signupUserName.text);

    }

    // L�gica para recuperar la contrase�a
    public void ForgetPass()
    {
        if (string.IsNullOrEmpty(forgetPassEmail.text))
        {
            ShowNotificationMessage("Error", "Please enter your email address.");
            return;
        }

        // Aqu� va la l�gica para restablecer la contrase�a
        forgetPasswordSubmit(forgetPassEmail.text);
    }

    // Mostrar mensaje de notificaci�n
    private void ShowNotificationMessage(string title, string message)
    {
        notif_Title_Text.text = "" + title;  // Mostrar el t�tulo
        notif_Message_Text.text = "" + message;  // Mostrar el mensaje

        notificationPanel.SetActive(true);  // Mostrar panel de notificaci�n
    }

    // Cerrar el panel de notificaci�n
    public void CloseNotifPanel()
    {
        notif_Title_Text.text = "";  // Limpiar t�tulo
        notif_Message_Text.text = "";  // Limpiar mensaje

        notificationPanel.SetActive(false);  // Ocultar panel de notificaci�n
    }

    // Cerrar sesi�n del usuario
    public void LogOut()
    {
        auth.SignOut();
        OpenLoginPanel();

        // Aqu� puedes agregar la l�gica para cerrar la sesi�n en Firebase

    }

    void CreateUser(string email, string password, string Username)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);

                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        ShowNotificationMessage("Error", GetErrorMessage(errorCode));
                    }
                }
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            UpdateUserProfile(Username);

            Debug.Log("Cambiando a la escena de Test");
            SceneManager.LoadScene("TestInformaci�n");
        });
    }


    public void SignInUser(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);

                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        ShowNotificationMessage("Error", GetErrorMessage(errorCode));
                    }
                }
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            // Acciones adicionales despu�s de iniciar sesi�n, como cambiar de escena
            ShowNotificationMessage("Alert", "Account Succesfully Created");

            // Cambiar a la escena de Men� principal (�ndice o nombre de la escena)
            SceneManager.LoadScene("MenuPrincipal 1");

        });
    }


    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
                && auth.CurrentUser.IsValid();
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                isSignIn = true;
            }
        }
    }

    void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
            auth = null;
        }
    }


    void UpdateUserProfile(string UserName)
{
    Debug.Log("Iniciando UpdateUserProfile");
    Firebase.Auth.FirebaseUser user = auth.CurrentUser;
    
    if (user != null)
    {
        Debug.Log("Actualizando el perfil del usuario: " + user.UserId);
        Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
        {
            DisplayName = UserName,
            PhotoUrl = new System.Uri("https://placehold.co/150"),
        };
        user.UpdateUserProfileAsync(profile).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("UpdateUserProfileAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                return;
            }

            Debug.Log("User profile updated successfully.");
            ShowNotificationMessage("Alert", "Account Successfully Created");

            
        });
    }
    else
    {
        Debug.LogError("No se puede actualizar el perfil: el usuario no est� autenticado.");
    }
}



    private static string GetErrorMessage(AuthError errorCode)
    {
        var message = "";
        switch (errorCode)
        {
            case AuthError.AccountExistsWithDifferentCredentials:
                message = "Ya existe la cuenta con credenciales diferentes";
                break;
            case AuthError.MissingPassword:
                message = "Hace falta el Password";
                break;
            case AuthError.WeakPassword:
                message = "El password es debil";
                break;
            case AuthError.WrongPassword:
                message = "El password es Incorrecto";
                break;
            case AuthError.EmailAlreadyInUse:
                message = "Ya existe la cuenta con ese correo electr�nico";
                break;
            case AuthError.InvalidEmail:
                message = "Correo electronico invalido";
                break;
            case AuthError.MissingEmail:
                message = "Hace falta el correo electr�nico";
                break;
            default:
                message = "Ocurri� un error";
                break;
        }
        return message;
    }

    //Logica para recuperar la contrase�a
    void forgetPasswordSubmit(string forgetPasswordEmail)
    {

        auth.SendPasswordResetEmailAsync(forgetPasswordEmail).ContinueWithOnMainThread(task => {

            if (task.IsCanceled)
            {
                Debug.LogError("SendPasswordResetEnaliAsync was canceled");
            }

            if (task.IsFaulted)
            {
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                    if (firebaseEx != null)
                    {
                        var errorCode = (AuthError)firebaseEx.ErrorCode;
                        ShowNotificationMessage("error", GetErrorMessage(errorCode));
                    }

                }

            }

            ShowNotificationMessage("Alert", "Se envio un mensaje a su email sobre el cambio de contrase�a");


        } 
        );


    }

}
