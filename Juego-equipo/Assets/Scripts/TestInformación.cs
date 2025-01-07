using UnityEngine;
using UnityEngine.SceneManagement; // Asegúrate de incluir esta línea para poder cambiar de escenas
using UnityEngine.UI; // Necesario para trabajar con el componente Button

public class TestInformación : MonoBehaviour
{
    public Button changeSceneButton; // Referencia al botón en el inspector

    // Start is called before the first frame update
    void Start()
    {
        // Asegúrate de que el botón no sea nulo y añade el listener
        if (changeSceneButton != null)
        {
            changeSceneButton.onClick.AddListener(ChangeScene);
        }
    }

    // Método para cambiar la escena
    void ChangeScene()
    {
        SceneManager.LoadScene("Test");
    }
}
