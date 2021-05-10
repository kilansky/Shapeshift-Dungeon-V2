﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class StartMenu : MonoBehaviour
{
    //public GameObject buttonsPanel;
    //public GameObject[] buttons;
    //public EventSystem eventSystem;

    [Header("Menus")]
    public GameObject startCanvas;
    public GameObject playCanvas;
    public GameObject optionsCanvas;
    public float menuTransitionTime;

    [Header("Assist Mode")]
    public Image assistCheckbox;
    public Sprite redX;
    public Sprite greenCheckmark;

    [Header("Settings")]
    public TextMeshProUGUI fullscreenText;

    [Header("Black Overlay")]
    public Image blackScreenOverlay;
    public float fadeInTime = 2f;
    public float fadeOutTime = 2f;

    private GameObject activeCanvas;
    private float startPos;
    private float endPos;

    // Start is called before the first frame update
    void Start()
    {
        startCanvas.SetActive(true);
        playCanvas.SetActive(false);

        activeCanvas = startCanvas;

        startPos = transform.position.y;
        endPos = transform.position.y - 6f;

        StartCoroutine(FadeInToMenu());
    }

    //Transitions to the play game canvas
    public void PlayGame()
    {
        StartCoroutine(TransitionMenu(playCanvas));
    }

    //Loads the game from floor 0
    public void StartFloor0()
    {
        PlayerPrefs.SetInt("startingLevel", 0);
        StartCoroutine(FadeOutToLevel(2));
    }

    public void StartFloor5()
    {
        PlayerPrefs.SetInt("startingLevel", 4);
        StartCoroutine(FadeOutToLevel(2));
    }

    public void StartFloor10()
    {
        PlayerPrefs.SetInt("startingLevel", 9);
        StartCoroutine(FadeOutToLevel(2));
    }

    public void StartFloor15()
    {
        PlayerPrefs.SetInt("startingLevel", 14);
        StartCoroutine(FadeOutToLevel(2));
    }

    public void StartFloor20()
    {
        PlayerPrefs.SetInt("startingLevel", 19);
        StartCoroutine(FadeOutToLevel(2));
    }

    //Transition to the options canvas
    public void Options()
    {
        StartCoroutine(TransitionMenu(optionsCanvas));
    }

    //Transition to the credits screen
    public void Credits()
    {
        StartCoroutine(FadeOutToLevel(3));
    }

    //Goes back to the start canvas
    public void Back()
    {
        StartCoroutine(TransitionMenu(startCanvas));
    }

    //Exits the games entirely
    public void QuitGame()
    {
        Application.Quit();
    }

    public void AssistMode()
    {
        //PlayerPrefs.GetInt("AssistMode")
    }

    //Toggle whether the game is fullscreen or not
    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        fullscreenText.text = Screen.fullScreen ? "Fullscreen" : "Windowed";
    }

    //Have the tile slide down & up to transition to the next menu
    private IEnumerator TransitionMenu(GameObject nextMenu)
    {
        float timeElapsed = 0;
        float currPos = startPos;

        CineShake.Instance.Shake(2f, (menuTransitionTime / 2));
        AudioManager.Instance.Play("MenuRumble");
        while (timeElapsed < (menuTransitionTime / 2))
        {
            currPos = Mathf.Lerp(startPos, endPos, timeElapsed / (menuTransitionTime / 2));
            transform.position = new Vector3(transform.position.x, currPos, transform.position.z);
            timeElapsed += Time.deltaTime;
            
            yield return new WaitForEndOfFrame();
        }
        transform.position = new Vector3(transform.position.x, endPos, transform.position.z);

        activeCanvas.SetActive(false);
        nextMenu.SetActive(true);
        activeCanvas = nextMenu;

        yield return new WaitForSeconds(0.25f);

        timeElapsed = 0;
        CineShake.Instance.Shake(2f, (menuTransitionTime / 2));
        AudioManager.Instance.Play("MenuRumble");
        while (timeElapsed < (menuTransitionTime / 2))
        {
            currPos = Mathf.Lerp(endPos, startPos, timeElapsed / (menuTransitionTime / 2));
            transform.position = new Vector3(transform.position.x, currPos, transform.position.z);
            timeElapsed += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        transform.position = new Vector3(transform.position.x, startPos, transform.position.z);
    }

    //Fade into the start menu
    private IEnumerator FadeInToMenu()
    {
        float alpha = 1;
        blackScreenOverlay.color = new Color(0, 0, 0, alpha);
        float timeElapsed = 0;
        
        while(timeElapsed < fadeInTime)
        {
            alpha = Mathf.Lerp(1, 0, timeElapsed / fadeInTime);
            blackScreenOverlay.color = new Color(0, 0, 0, alpha);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        blackScreenOverlay.color = new Color(0, 0, 0, 0);
    }

    //Fade out to load a specified scene
    private IEnumerator FadeOutToLevel(int sceneToLoad)
    {
        float alpha = 0;
        blackScreenOverlay.color = new Color(0, 0, 0, alpha);
        float timeElapsed = 0;

        while (timeElapsed < fadeOutTime)
        {
            alpha = Mathf.Lerp(0, 1, timeElapsed / fadeOutTime);
            blackScreenOverlay.color = new Color(0, 0, 0, alpha);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        blackScreenOverlay.color = new Color(0, 0, 0, 1);
        yield return new WaitForEndOfFrame();

        SceneManager.LoadScene(sceneToLoad);
    }
}
