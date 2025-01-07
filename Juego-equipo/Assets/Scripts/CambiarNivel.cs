using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class CambiarNivel : MonoBehaviour
{
    public void CambiarEscena(string nombre)
    {
        // si existe que haga el aumentar el niveles (nota el unico que tendra el gameobject de controlador niveles es el submenu (ya con sus botones y en el otro apartado lo tiene en 0)
        // y en todos los niveles 1 - 7 tienen el controladorNiveles pero no agregan botones solo agrengan la variable del nivel a desbloquear pero 
        // nota este cambiarNivel tambien lo tiene todos los niveles del 1 - 7. para pues cambiar de escena de los niveles.
        if(ControladorNiveles.instancia != null)
        {
            ControladorNiveles.instancia.AumentarNiveles();
        }
 
        SceneManager.LoadScene(nombre);
    }
}
