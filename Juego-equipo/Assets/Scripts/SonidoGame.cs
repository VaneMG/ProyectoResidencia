using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SonidoGame : MonoBehaviour
{
    private AudioSource music;
    public AudioClip ClikAudio;

    void star()
    {
        music = GetComponent<AudioSource>();
    }


    public void ClickAudioOn()
    {
        music.PlayOneShot(ClikAudio);
    }
}
