using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicaGame : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // permite que la musica suena
    [SerializeField] private AudioClip mainMenuMusic;// visualiza la musica para 4 menus
    [SerializeField] private AudioClip playSceneMusic; // Música para niveles 1 y 2
    [SerializeField] private AudioClip playSceneMusicDos; // Música para niveles 3, 4, 5
    [SerializeField] private AudioClip playSceneMusicTres; // Música para niveles 6 y 7

    private static MusicaGame instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateMusic();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateMusic();
    }

    private void UpdateMusic()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex == 4 || currentSceneIndex == 8) // Niveles 1 y 2
        {
            audioSource.clip = playSceneMusic;
            Debug.Log("Música de niveles 1 y 2 asignada");
        }
        else if (currentSceneIndex == 9 || currentSceneIndex == 10 || currentSceneIndex == 11) // Niveles 3, 4, 5
        {
            audioSource.clip = playSceneMusicDos;
            Debug.Log("Música de niveles 3, 4 y 5 asignada");
        }
        else if (currentSceneIndex == 12 || currentSceneIndex == 13) // Niveles 6 y 7
        {
            audioSource.clip = playSceneMusicTres;
            Debug.Log("Música de niveles 6 y 7 asignada");
        }
        else // Escenas que no son niveles
        {
            audioSource.clip = mainMenuMusic;
            Debug.Log("Música del menú principal asignada");
        }

        audioSource.Play();
    }

}




