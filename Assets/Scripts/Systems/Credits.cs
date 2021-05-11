using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class Credits : MonoBehaviour
{
    [Header("Credits")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI producedByText;
    public TextMeshProUGUI thankYouText;
    public Image heartPotion;
    public float titleFadeTime = 2.5f;
    public float subtitleFadeTime = 1.5f;
    public float holdOnTitleTime = 2f;
    public float scrollSpeed = 1f;
    public float timeToStopCredits = 100f;

    [Header("Skip")]
    public PlayerInput playerInput;
    public GameObject skipButtonPanel;
    public Image skipButtonImage;
    public Sprite keyboardSkipButton;
    public Sprite controllerSkipButton;

    private bool isScrolling = false;
    private string currentControlScheme;

    void Start()
    {
        Time.timeScale = 1;
        skipButtonPanel.SetActive(false);

        StartCoroutine(FadeTitleFromBlack());
        StartCoroutine(WaitToStop());

        ControlSchemeChanged();
    }

    void Update()
    {
        if(isScrolling)
            transform.position += new Vector3(0, 1, 0) * scrollSpeed * Time.deltaTime;

        if (playerInput.currentControlScheme != currentControlScheme)
            ControlSchemeChanged();
    }

    //Change the UI based on the new control type
    public void ControlSchemeChanged()
    {
        currentControlScheme = playerInput.currentControlScheme;
        Debug.Log("currentControlScheme: " + currentControlScheme);

        //Using Keyboard
        if (playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            //Set Cursor to be visible
            Cursor.visible = true;

            //Update button icon
            skipButtonImage.sprite = keyboardSkipButton;
        }
        //Using Controller
        else
        {
            //Set Cursor to not be visible
            Cursor.visible = false;

            //Update button icon
            skipButtonImage.sprite = controllerSkipButton;
        }
    }

    //Show the skip button when any button is pressed
    public void AnyButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
            skipButtonPanel.SetActive(true);
    }

    //Skip the credits and load main menu
    public void SkipButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed && skipButtonPanel.activeSelf)
            SceneManager.LoadScene(0);
    }

    private IEnumerator WaitToScroll()
    {
        yield return new WaitForSeconds(holdOnTitleTime);
        isScrolling = true;
    }

    private IEnumerator WaitToStop()
    {
        yield return new WaitForSeconds(timeToStopCredits);
        isScrolling = false;

        StartCoroutine(FadeToBlack());
    }

    //Fade from black into the title
    private IEnumerator FadeTitleFromBlack()
    {
        float alpha = 0;
        float fadeInTime = titleFadeTime;
        titleText.color = new Color(1, 1, 1, alpha);
        float timeElapsed = 0;

        while (timeElapsed < fadeInTime)
        {
            alpha = Mathf.Lerp(0, 1, timeElapsed / fadeInTime);
            titleText.color = new Color(1, 1, 1, alpha);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        titleText.color = new Color(1, 1, 1, 1);

        yield return new WaitForEndOfFrame();

        StartCoroutine(FadeSubtitleFromBlack());
    }

    //Fade from black into the title
    private IEnumerator FadeSubtitleFromBlack()
    {
        float alpha = 0;
        float fadeInTime = titleFadeTime;
        producedByText.color = new Color(1, 1, 1, alpha);
        float timeElapsed = 0;

        while (timeElapsed < fadeInTime)
        {
            alpha = Mathf.Lerp(0, 1, timeElapsed / fadeInTime);
            producedByText.color = new Color(1, 1, 1, alpha);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        producedByText.color = new Color(1, 1, 1, 1);

        yield return new WaitForEndOfFrame();

        StartCoroutine(WaitToScroll());
    }


    //Fade to black and go back to menu screen
    private IEnumerator FadeToBlack()
    {
        float alpha = 1;
        float fadeInTime = titleFadeTime;
        thankYouText.color = new Color(1, 1, 1, alpha);
        heartPotion.color = new Color(1, 1, 1, alpha);
        float timeElapsed = 0;

        while (timeElapsed < fadeInTime)
        {
            alpha = Mathf.Lerp(1, 0, timeElapsed / fadeInTime);
            thankYouText.color = new Color(1, 1, 1, alpha);
            heartPotion.color = new Color(1, 1, 1, alpha);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        thankYouText.color = new Color(1, 1, 1, 0);
        heartPotion.color = new Color(1, 1, 1, 0);

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(0);
    }
}
