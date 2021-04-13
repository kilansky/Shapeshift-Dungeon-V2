using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//Manages the background music in the game
public class MusicManager : SingletonPattern<MusicManager>
{
    public AudioClip startMenu;
    public AudioClip combat1;
    public AudioClip combat2;
    public AudioClip combat3;
    public AudioClip floorCleared1;
    public AudioClip floorCleared2;
    public AudioClip floorCleared3;
    public AudioClip shop;

    private AudioSource ac;

    private void Start()
    {
        ac = GetComponent<AudioSource>();
        FloorCleared1();
    }

    public void StartMenu()
    {
        STOP();
        ac.PlayOneShot(startMenu);
    }

    public void Combat1()
    {
        STOP();
        ac.PlayOneShot(combat1);
    }

    public void Combat2()
    {
        STOP();
        ac.PlayOneShot(combat2);
    }

    public void Combat3()
    {
        STOP();
        ac.PlayOneShot(combat3);
    }

    public void FloorCleared1()
    {
        STOP();
        ac.PlayOneShot(floorCleared1);
    }

    public void FloorCleared2()
    {
        STOP();
        ac.PlayOneShot(floorCleared2);
    }

    public void FloorCleared3()
    {
        STOP();
        ac.PlayOneShot(floorCleared3);
    }

    public void Shop()
    {
        STOP();
        ac.PlayOneShot(shop);
    }

    public void STOP()
    {
        ac.Stop();
    }
}
