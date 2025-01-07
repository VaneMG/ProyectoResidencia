using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumenGame : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    // Método para ajustar el volumen de la música
    public void ControlMusicVolume(float sliderValue)
    {
        audioMixer.SetFloat("volumenmusica", Mathf.Log10(sliderValue) * 20);
    }
}
