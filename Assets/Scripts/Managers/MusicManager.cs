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
    public float volume = 0.6f;

    private AudioSource ac;

    private void Start()
    {
        ac = GetComponent<AudioSource>();
        FloorCleared1();
    }

    [ContextMenu("StartMenu")]
    public void StartMenu()
    {
        STOP();
        ac.PlayOneShot(startMenu, volume);
    }

    [ContextMenu("Combat1")]
    public void Combat1()
    {
        STOP();
        ac.PlayOneShot(combat1, volume);
    }

    [ContextMenu("Combat2")]
    public void Combat2()
    {
        STOP();
        ac.PlayOneShot(combat2, volume);
    }

    [ContextMenu("Combat3")]
    public void Combat3()
    {
        STOP();
        ac.PlayOneShot(combat3, volume);
    }

    [ContextMenu("FloorCleared1")]
    public void FloorCleared1()
    {
        STOP();
        ac.PlayOneShot(floorCleared1, volume);
    }

    [ContextMenu("FloorCleared2")]
    public void FloorCleared2()
    {
        STOP();
        ac.PlayOneShot(floorCleared2, volume);
    }

    [ContextMenu("FloorCleared3")]
    public void FloorCleared3()
    {
        STOP();
        ac.PlayOneShot(floorCleared3, volume);
    }

    [ContextMenu("Shop")]
    public void Shop()
    {
        STOP();
        ac.PlayOneShot(shop, volume);
    }

    public void STOP()
    {
        ac.Stop();
    }
}
