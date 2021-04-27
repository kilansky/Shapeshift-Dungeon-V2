using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//Manages the background music in the game
public class MusicManager : SingletonPattern<MusicManager>
{
    public AudioClip exoticDangers;
    public AudioClip dungeonDecoration;
    public AudioClip dungeonDestruction;
    public AudioClip floorCleared;
    public AudioClip shop;
    public float volume = 0.6f;

    private AudioSource ac;

    private void Start()
    {
        ac = GetComponent<AudioSource>();
        ac.volume = volume;
    }

    [ContextMenu("Combat")]
    public void ExoticDangers()
    {
        STOP();
        ac.clip = exoticDangers;
        ac.volume = volume;
        ac.Play();
    }

    public void DungeonDecoration()
    {
        STOP();
        ac.clip = dungeonDecoration;
        ac.volume = volume;
        ac.Play();
    }

    public void DungeonDestruction()
    {
        STOP();
        ac.clip = dungeonDestruction;
        ac.volume = volume;
        ac.Play();
    }

    [ContextMenu("FloorCleared")]
    public void FloorCleared()
    {
        STOP();
        ac.volume = 0f;
        FadeIn(volume, 1f);
        ac.clip = floorCleared;
        ac.volume = volume;
        ac.Play();
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
        //Debug.Log(originalVolume);
        while (counter < duration) //This while loop moves the object to new position over a set amount of time
        {
            counter += Time.deltaTime;
            float newVol = Mathf.Lerp(originalVolume, end, counter / duration);
            //Debug.Log(newVol);
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
        //Debug.Log(originalVolume);
        while (counter < duration) //This while loop moves the object to new position over a set amount of time
        {
            counter += Time.deltaTime;
            float newVol = Mathf.Lerp(end, originalVolume, counter / duration);
            //Debug.Log(newVol);
            ac.volume = originalVolume - newVol;
            yield return null;
        }
    }
}
