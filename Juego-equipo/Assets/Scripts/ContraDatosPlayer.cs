using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using System;

public class ContraDatosPlayer : MonoBehaviour
{
    public static ContraDatosPlayer Instance { get; private set; } // Singleton

    public Reloj reloj;
    public GameObject jugador;
    public string archivoDeGuardado;
    public DatosJuego datosJuego = new DatosJuego();
    private vidasPlay vidaJugador;  // Referencia al script de vida del jugador

    public string nivelActual;  // Puedes cambiar este valor dependiendo del nivel actual

    private FirebaseFirestore firestore;

    private void Awake()
    {
        // Asignar la referencia al reloj al inicio
        reloj = FindObjectOfType<Reloj>();
        if (reloj == null)
        {
            Debug.LogError("No se encontró el objeto Reloj al iniciar.");
        }
        else
        {
            // Verificar si la escena actual es una escena de nivel
            string escenaActual = SceneManager.GetActiveScene().name;

            if (EsEscenaDeNivel(escenaActual))
            {
                DontDestroyOnLoad(reloj.gameObject); // Solo hacer persistente si es un nivel
            }
            else
            {
                Destroy(reloj.gameObject); // Destruye el reloj en escenas no permitidas
                Destroy(gameObject); // También destruir el GameObject que contiene ContraDatosPlayer
                return; // Salir para evitar marcarlo como DontDestroyOnLoad
            }
        }

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Asegura que el objeto no se destruya al cambiar de escena
        }

        // Asignación de referencias jugador y vidaJugador
        if (jugador == null)
        {
            jugador = GameObject.FindGameObjectWithTag("Player");
        }

        if (jugador != null)
        {
            vidaJugador = jugador.GetComponent<vidasPlay>();
            if (vidaJugador == null)
            {
                Debug.LogError("El componente vidasPlay no se encontró en el jugador.");
            }
        }
        else
        {
            Debug.LogError("No se encontró ningún objeto con el tag 'Player'.");
        }

        // Asignar la ruta del archivo de guardado
        archivoDeGuardado = Application.dataPath + "/datosJuego.json";

        // Inicializar Firestore
        firestore = FirebaseFirestore.DefaultInstance;

        // Cargar los datos automáticamente al iniciar
        //CargarDatos();
    }

    private void Start()
    {
        // Suscribirse al evento de cambio de escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private bool EsEscenaDeNivel(string escenaNombre)
    {
        // Lista de nombres de escenas de niveles (ajusta los nombres según tu proyecto)
        List<string> niveles = new List<string> { "Juego", "Nivel 2", "Nivel 3", "Nivel 4", "Nivel 5", "Nivel 6", "Nivel 7" };
        return niveles.Contains(escenaNombre);
    }

    private void OnDestroy()
    {
        // Desuscribirse para evitar referencias inválidas
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Actualiza nivelActual con el nombre de la nueva escena
        nivelActual = scene.name;

        // Destruir ContraDatosPlayer si la escena no es un nivel válido
        if (!EsEscenaDeNivel(nivelActual))
        {
            Destroy(gameObject); // Asegura que el objeto no persista en escenas no válidas
            return; // Salir del método si no es una escena válida
        }


        // Reinicia el puntaje y el tiempo al cargar un nuevo nivel
        if (PuntosGuargar.Instance != null)
        {
            PuntosGuargar.Instance.ReiniciarPuntajeYMonedas();
        }

        // Cargar los datos del jugador para el nuevo nivel
        CargarDatos();

        // Verifica si la escena cargada es un nivel
        if (reloj != null)
        {
            if (EsEscenaDeNivel(scene.name))
            {
                reloj.ReiniciarReloj(); // Reinicia el reloj al cargar un nuevo nivel
            }
            else
            {
                Destroy(reloj.gameObject); // Destruye el reloj si no es un nivel
            }
        }
    }
    // Método público para asignar el valor
    public void AsignarVidaJugador(vidasPlay nuevaVidaJugador)
    {
        vidaJugador = nuevaVidaJugador;
    }

    private void CargarDatos()
    {
        // Verifica si el archivo existe antes de intentar cargar
        if (File.Exists(archivoDeGuardado))
        {
            string contenido = File.ReadAllText(archivoDeGuardado);
            datosJuego = JsonUtility.FromJson<DatosJuego>(contenido);

            // Verificar si el puntaje del nivel actual está guardado
            if (datosJuego.puntajesPorNivel.ContainsKey(nivelActual))
            {
                PuntosGuargar.Instance.SetPuntaje(datosJuego.puntajesPorNivel[nivelActual]);
            }
            else
            {
                datosJuego.puntajesPorNivel[nivelActual] = 0; // Asignar valor por defecto si no existe
            }

            // Cargar posición y vida
            if (jugador != null && vidaJugador != null)
            {
                vidaJugador.vidaActual = datosJuego.vidaActual;  // Cargar vida actual
                vidaJugador.vidaMaxima = datosJuego.vidaMaxima;  // Cargar vida máxima
            }

            if (datosJuego. monedasPorNivel.ContainsKey(nivelActual))
            {
                PuntosGuargar.Instance.SetTotalMonedas(datosJuego.monedasPorNivel[nivelActual]);
            }
            else
            {
                datosJuego.monedasPorNivel[nivelActual] = 0; // Asignar valor por defecto si no existe
            }

            Debug.Log("Datos cargados. Puntos " + nivelActual + ": " + datosJuego.puntajesPorNivel[nivelActual] +
                      " Vida actual: " + datosJuego.vidaActual + "cantidad de moneda" + datosJuego.monedasPorNivel[nivelActual]);
        }
        else
        {
            Debug.Log("El archivo no existe. Se crearán nuevos datos al guardar.");
        }
    }


    public void GuardarDatos(string uid)
    {
        if (reloj == null)
        {
            Debug.LogError("No se encontró el objeto Reloj al guardar datos.");
            return; // Salir si el reloj no está disponible
        }

        // Reasignar el jugador y el componente vidaJugador en caso de que sean nulos
        if (jugador == null)
        {
            jugador = GameObject.FindGameObjectWithTag("Player");
        }

        if (jugador != null)
        {
            vidaJugador = jugador.GetComponent<vidasPlay>();
        }

        // Verificar si el jugador y el componente vidaJugador existen antes de intentar guardar
        if (jugador == null)
        {
            Debug.LogError("No se encontró ningún objeto con el tag 'Player'.");
            return; // Salir si no se encuentra al jugador
        }

        if (vidaJugador == null)
        {
            Debug.LogError("El componente vidasPlay no se encontró en el jugador.");
            return; // Salir si no se encuentra el componente de vida
        }

        // Guardar el personaje seleccionado en los datos del jugador
        datosJuego.personajeSeleccionado = "id_del_personaje_seleccionado"; // Aquí puedes obtener el ID del personaje seleccionado

        // Guardar datos
        datosJuego.tiempoDeJuego = reloj.GetTiempoTranscurrido();

        // Guardar el UID del usuario
        datosJuego.uid = uid;

        // Asegurarse de que el diccionario tenga la clave del nivel antes de guardar
        if (datosJuego.puntajesPorNivel.ContainsKey(nivelActual))
        {
            datosJuego.puntajesPorNivel[nivelActual] = PuntosGuargar.Instance.GetPuntaje();
        }
        else
        {
            datosJuego.puntajesPorNivel.Add(nivelActual, PuntosGuargar.Instance.GetPuntaje());
        }

        if (datosJuego.monedasPorNivel.ContainsKey(nivelActual))
        {
            datosJuego.monedasPorNivel[nivelActual] = PuntosGuargar.Instance.GetTotalMonedas();
        }
        else
        {
            datosJuego.monedasPorNivel.Add(nivelActual, PuntosGuargar.Instance.GetTotalMonedas());
        }

        // Guardar la posición, puntaje y vida del jugador
        datosJuego.vidaActual = vidaJugador.vidaActual;
        datosJuego.vidaMaxima = vidaJugador.vidaMaxima;

        // Guardar en el archivo
        string cadenaJSON = JsonUtility.ToJson(datosJuego);
        File.WriteAllText(archivoDeGuardado, cadenaJSON);

        // Guardar en Firestore
        GuardarEnFirestore(uid);

        Debug.Log("Datos guardados. Puntos " + nivelActual + ": " + datosJuego.puntajesPorNivel[nivelActual] +
                  " UID: " + uid + " Vida actual: " + datosJuego.vidaActual +
                  " Tiempo de juego: " + datosJuego.tiempoDeJuego + " segundos " + " Total monedas:" + datosJuego.monedasPorNivel[nivelActual]);
    }

    private void GuardarEnFirestore(string uid)
    {
        // Usar una combinación de uid y nivelActual como ID del documento
        string documentoID = $"{uid}_{nivelActual}";

        DocumentReference progresoRef = firestore.Collection("ProgresoUsuario").Document(documentoID); // Crear un documento único para cada nivel de usuario
        //DocumentReference estadisticasRef = firestore.Collection("EstadisticasJuego").Document(uid);
        DocumentReference monedasRef = firestore.Collection("Monedas").Document(uid); // Referencia para la colección de monedas

        // Primero, obtenemos los datos actuales guardados en Firestore para comparar
        progresoRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error al obtener los resultados existentes: " + task.Exception);
                return;
            }

            var snapshot = task.Result;
            bool esPrimeraVez = !snapshot.Exists; // Determina si es la primera vez que se guarda el progreso
            bool actualizarProgreso = false;
            bool actualizarMonedas = false;

            try
            {
                // Datos a guardar inicialmente
                var datosProgreso = new Dictionary<string, object>
            {
                { "user_id", uid },
                { "nivel_id", nivelActual },
                { "puntuacion_actual", datosJuego.puntajesPorNivel[nivelActual] },
                { "total_monedas_actual", datosJuego.monedasPorNivel[nivelActual] },
                { "tiempo_juego", datosJuego.tiempoDeJuego },
                { "vidas_jugador", datosJuego.vidaActual },
                { "fecha_completado", DateTime.UtcNow.ToString("o") } // Fecha en formato ISO 8601
            };

                if (esPrimeraVez)
                {
                    // Guardar los datos iniciales si es la primera vez
                    progresoRef.SetAsync(datosProgreso).ContinueWithOnMainThread(saveTask =>
                    {
                        if (saveTask.IsCompleted)
                        {
                            Debug.Log("Progreso inicial guardado exitosamente en Firestore.");
                        }
                        else
                        {
                            Debug.LogError("Error al guardar progreso inicial en Firestore: " + saveTask.Exception);
                        }
                    });
                }
                else if (snapshot.ContainsField("tiempo_juego"))
                {
                    // Comparar tiempos si el documento ya existe
                    float tiempoGuardado = snapshot.GetValue<float>("tiempo_juego");

                    if (datosJuego.tiempoDeJuego < tiempoGuardado)
                    {
                        // Actualizar únicamente si el tiempo es menor
                        actualizarProgreso = true;
                        actualizarMonedas = true;
                        string mensaje = $"¡Increíble! Has completado el nivel en un tiempo menor: {datosJuego.tiempoDeJuego:F2}s.";
                        Debug.Log(mensaje);

                        if (GanarJuego.Instance != null)
                        {
                            GanarJuego.Instance.ActualizarMensaje(mensaje);
                        }

                        // Añadir campos actualizados al documento existente
                        var datosActualizados = new Dictionary<string, object>
                    {
                        { "puntuacion_actualizada", datosJuego.puntajesPorNivel[nivelActual] },
                        { "total_monedas_actualizadas", datosJuego.monedasPorNivel[nivelActual] },
                        { "vidas_actualizadas", datosJuego.vidaActual },
                        { "tiempo_actualizado", datosJuego.tiempoDeJuego }, // Guardar tiempo actualizado
                        { "fecha_actualizada", DateTime.UtcNow.ToString("o") } // Fecha de actualización
                    };

                        progresoRef.UpdateAsync(datosActualizados).ContinueWithOnMainThread(updateTask =>
                        {
                            if (updateTask.IsCompleted)
                            {
                                Debug.Log("Datos actualizados exitosamente en Firestore.");
                            }
                            else
                            {
                                Debug.LogError("Error al actualizar datos en Firestore: " + updateTask.Exception);
                            }
                        });
                    }
               
                else
                    {
                        string mensaje = "¡Buen intento! Intenta superar tu mejor tiempo la próxima vez.";
                        Debug.Log(mensaje);

                        if (GanarJuego.Instance != null)
                        {
                            GanarJuego.Instance.ActualizarMensaje(mensaje);
                        }
                    }
                }

                // Si las condiciones se cumplen, actualizar también las monedas
                // Aquí puedes omitir o manejar el código de actualización de monedas como lo tenías previamente
                if (actualizarMonedas)
                {
                    GuardarMonedasEnColeccion(uid, datosJuego.puntajesPorNivel[nivelActual]);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error al leer valores del documento existente: " + e.Message);
            }
        });
    }

    public void GuardarMonedasEnColeccion(string uid, int puntuacionObtenida)
    {
        DocumentReference monedasRef = firestore.Collection("Monedas").Document(uid);

        monedasRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error al obtener las monedas del usuario en Firestore: " + task.Exception);
                return;
            }

            int monedasTotales = 0; // Inicia el contador de monedas desde 0

            if (task.Result.Exists)
            {
                try
                {
                    // Leer el total acumulado solo si el documento ya existe
                    monedasTotales = task.Result.GetValue<int>("monedas_totales");
                }
                catch (Exception e)
                {
                    Debug.LogError("Error al leer el valor del documento existente: " + e.Message);
                }
            }

            // Acumular únicamente el puntaje obtenido
            monedasTotales += puntuacionObtenida;

            // Asegurarse de que las monedas no sean negativas
            if (monedasTotales < 0)
            {
                Debug.LogWarning("No hay suficientes monedas para completar la transacción.");
                return;
            }


            // Preparar datos para guardar en Firestore
            var datosMonedas = new Dictionary<string, object>
        {
            { "user_id", uid },
            { "monedas_totales", monedasTotales },
            { "ultima_actualizacion", DateTime.UtcNow.ToString("o") }
        };

            // Guardar los datos actualizados
            monedasRef.SetAsync(datosMonedas).ContinueWithOnMainThread(saveTask =>
            {
                if (saveTask.IsCompleted)
                {
                    Debug.Log($"Monedas actualizadas exitosamente: Total = {monedasTotales}");
                }
                else
                {
                    Debug.LogError("Error al actualizar monedas en Firestore: " + saveTask.Exception);
                }
            });
        });
    }


    public void GuardarDatosEnSubmenu(string uid)
    {
        GuardarDatos(uid);
    }
}