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
        ac.volume = volume;
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
        ac.clip = combat2;
        ac.volume = volume;
        ac.Play();
        //ac.PlayOneShot(combat2, volume);
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
        ac.volume = 0f;
        FadeIn(volume, 1f);
        ac.clip = floorCleared2;
        ac.volume = volume;
        ac.Play();
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
        ac.clip = shop;
        ac.volume = volume;
        ac.Play();
    }

    public void STOP()
    {
        ac.Stop();
    }

    public void FadeIn(float newValue, float duration)
    {
        StartCoroutine(FaderIn(newValue, duration));
    }

    private IEnumerator FaderIn(float end, float duration)
    {
        float counter = 0f; //Counter to keep track of time elapsed
        float originalVolume = ac.volume;
        Debug.Log(originalVolume);
        while (counter < duration) //This while loop moves the object to new position over a set amount of time
        {
            counter += Time.deltaTime;
            float newVol = Mathf.Lerp(originalVolume, end, counter / duration);
            Debug.Log(newVol);
            ac.volume = originalVolume + newVol;
            yield return null;
        }
    }

    public void FadeOut(float newValue, float duration)
    {
        StartCoroutine(FaderOut(newValue, duration));
    }

    private IEnumerator FaderOut(float end, float duration)
    {
        float counter = 0f; //Counter to keep track of time elapsed
        float originalVolume = ac.volume;
        Debug.Log(originalVolume);
        while (counter < duration) //This while loop moves the object to new position over a set amount of time
        {
            counter += Time.deltaTime;
            float newVol = Mathf.Lerp(end, originalVolume, counter / duration);
            Debug.Log(newVol);
            ac.volume = originalVolume - newVol;
            yield return null;
        }
    }
}
