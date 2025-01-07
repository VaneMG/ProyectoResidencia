using UnityEditor;

public static class BuildScript
{
    public static void BuildiOS()
    {
        string[] scenes = { "Assets/FirebaseAuth.unity", "Assets/MenuPrincipal1.unity", "Assets/MenuPrincipal1.unity", "Assets/Submenu.unity", "Assets/Juego.unity" , "Assets/Nivel 2.unity", "Assets/Nivel 3.unity", "Assets/Nivel 4.unity", "Assets/Nivel 5.unity", "Assets/Nivel 6.unity", "Assets/Nivel 7.unity", "Assets/Avatar.unity", "Assets/PanelJuego.unity", "Assets/PerfilUsuario.unity", "Assets/SeleccionPersonaje.unity", "Assets/Test.unity", "Assets/Nivel 5.unity", "Assets/Nivel 6.unity", "Assets/Nivel 7.unity", "Assets/Avatar.unity", "Assets/PanelJuego.unity", "Assets/PerfilUsuario.unity", "Assets/SeleccionPersonaje.unity", "Assets/TestInfo.unity" }; // Reemplaza con tus escenas
        string buildPath = "build/ios";
        BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.iOS, BuildOptions.None);
    }
}
