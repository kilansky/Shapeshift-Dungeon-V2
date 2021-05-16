using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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

    [Header("Menus")]
    public Button floor5;
    public Button floor10;
    public Button floor15;
    public Button floor20;

    [Header("Difficulty")]
    public TextMeshProUGUI difficultyButtonText;
    public GameObject casualDiamonds;
    public GameObject standardDiamonds;
    public GameObject hardcoreDiamonds;

    [Header("Settings")]
    public TextMeshProUGUI fullscreenText;
    public AudioMixer mixer;

    private AudioSource audioSource;
    private PlayerInput playerInput;
    private float currSliderValue;
    private bool isUsingMouse;

    public void SetMusicLevel(float sliderValue)
    {
        if(sliderValue > 0)
            mixer.SetFloat("MusicVolume", (Mathf.Log10(sliderValue) * 20) + 5);
        else
            mixer.SetFloat("MusicVolume", -80);
    }

    public void SetSFXLevel(float sliderValue)
    {
        if (sliderValue > 0)
            mixer.SetFloat("SFXVolume", (Mathf.Log10(sliderValue) * 20) + 5);
        else
            mixer.SetFloat("SFXVolume", -80);

        currSliderValue = sliderValue;

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
            StartCoroutine(StopRumbleSFX());
        }
    }

    [Header("Black Overlay")]
    public Image blackScreenOverlay;
    public float fadeInTime = 2f;
    public float fadeOutTime = 2f;

    private GameObject activeCanvas;
    private string currentControlScheme;
    private float startPos;
    private float endPos;

    private Slider mySlider;
    private GameObject thisSlider;
    private float sliderChange;
    private float maxSliderValue;
    private float minSliderValue;
    private float sliderRange;
    private const float SLIDERSTEP = 100.0f; //used to detrime how fine to change value
    private float sliderNavigation;
    //private const string SLIDERMOVE = "SliderHorizontal";

    //Initialize values
    private void SetSlider()
    {
        mySlider = EventSystem.current.currentSelectedGameObject.GetComponent<Slider>();
        thisSlider = EventSystem.current.currentSelectedGameObject;
        maxSliderValue = mySlider.maxValue;
        minSliderValue = mySlider.minValue;
        sliderRange = maxSliderValue - minSliderValue;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        startCanvas.SetActive(true);
        playCanvas.SetActive(false);

        activeCanvas = startCanvas;

        startPos = transform.position.y;
        endPos = transform.position.y - 6f;

        //set preferred difficulty and screen size     
        SetPreferredScreenSize();
        SetPreferredDifficulty();

        DisableLockedFloors();

        playerInput = GetComponent<PlayerInput>();
        currentControlScheme = playerInput.currentControlScheme;

        if (currentControlScheme == "Keyboard&Mouse")
        {
            isUsingMouse = true;

            //Set Cursor to be visible
            Cursor.visible = true;

            //Clear selected buttons
            GetComponent<EventSystem>().SetSelectedGameObject(null);
            startCanvas.GetComponent<Buttons>().ClearSelectedButtons();
        }
        else
        {
            isUsingMouse = false;

            //Set Cursor to not be visible
            Cursor.visible = false;

            //Set selected button
            startCanvas.GetComponent<Buttons>().SetSelectedButton();
        }

        StartCoroutine(FadeInToMenu());
    }

    private void Update()
    {
        if (playerInput.currentControlScheme != currentControlScheme)
            ControlSchemeChanged();

        Cursor.visible = isUsingMouse ? true : false;

        //If slider has 'focus'
        if (thisSlider && thisSlider == EventSystem.current.currentSelectedGameObject)
        {
            sliderChange = sliderNavigation * sliderRange / SLIDERSTEP;
            float sliderValue = mySlider.value;
            float tempValue = sliderValue + sliderChange;
            if (tempValue <= maxSliderValue && tempValue >= minSliderValue)
            {
                sliderValue = tempValue;
            }
            mySlider.value = sliderValue;
        }
    }

    private void ControlSchemeChanged()
    {
        //Set initial selected button if using controller
        if (playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            isUsingMouse = true;
            activeCanvas.GetComponent<Buttons>().ClearSelectedButtons();
        }
        else
        {
            isUsingMouse = false;
            activeCanvas.GetComponent<Buttons>().SetSelectedButton();
        }

        currentControlScheme = playerInput.currentControlScheme;
    }

    private void SetPreferredScreenSize()
    {
        if (PlayerPrefs.GetInt("isFullscreen", 0) == 0)
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
            fullscreenText.text = "Fullscreen";
        }
        else
        {
            Screen.SetResolution(1024, 768, false);
            fullscreenText.text = "Windowed";
        }
    }

    private void SetPreferredDifficulty()
    {
        casualDiamonds.SetActive(false);
        standardDiamonds.SetActive(false);
        hardcoreDiamonds.SetActive(false);

        switch (PlayerPrefs.GetInt("Difficulty", 1))
        {
            case 0: //assist
                difficultyButtonText.text = "Casual";
                casualDiamonds.SetActive(true);
                PlayerPrefs.GetInt("Difficulty", 0);
                break;
            case 1: //standard
                difficultyButtonText.text = "Standard";
                standardDiamonds.SetActive(true);
                PlayerPrefs.GetInt("Difficulty", 1);
                break;
            case 2: //hardcore
                difficultyButtonText.text = "Hardcore";
                hardcoreDiamonds.SetActive(true);
                PlayerPrefs.GetInt("Difficulty", 2);
                break;
            default:
                break;
        }
    }

    private IEnumerator StopRumbleSFX()
    {
        float tempValue = currSliderValue;
        yield return new WaitForSeconds(0.5f);

        if(currSliderValue == tempValue)
            audioSource.Stop();

        if(audioSource.isPlaying)
            StartCoroutine(StopRumbleSFX());
    }

    //Set movementVector based on movement input
    public void Navigate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 navigationInput = context.ReadValue<Vector2>();

            Buttons buttons = FindObjectOfType<Buttons>();

            if(!EventSystem.current.currentSelectedGameObject)
                buttons.SetSelectedButton();

            if (EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.GetComponent<Slider>())
            {
                SetSlider();
                sliderNavigation = Mathf.Clamp(navigationInput.x, -1, 1);

                if (navigationInput.y > 0.5f)
                    buttons.PreviousButton();

                if (navigationInput.y < -0.5f)
                    buttons.NextButton();
            }
            else if (buttons)
            {
                if (navigationInput.y > 0.5f || navigationInput.x < -0.5f)
                    buttons.PreviousButton();

                if (navigationInput.y < -0.5f || navigationInput.x > 0.5f)
                    buttons.NextButton();
            }
        }
        if(context.canceled)
        {
            sliderNavigation = 0;
        }
    }

    //Dash Button Pressed
    public void Submit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Buttons buttons = FindObjectOfType<Buttons>();

            if (buttons && !EventSystem.current.currentSelectedGameObject)
                buttons.SetSelectedButton();

            if (buttons)
                buttons.SubmitButton();
        }
    }

    //Attack Button Pressed
    public void Cancel(InputAction.CallbackContext context)
    {
        if (context.performed && activeCanvas != startCanvas)
        {
            StartCoroutine(TransitionMenu(startCanvas));
        }
    }

    //Transitions to the play game canvas
    public void PlayGame()
    {
        StartCoroutine(TransitionMenu(playCanvas));
    }

    public void DisableLockedFloors()
    {
        floor5.interactable = false;
        floor10.interactable = false;
        floor15.interactable = false;
        floor20.interactable = false;

        if (PlayerPrefs.GetInt("unlockedLevels", 0) >= 5)
            floor5.interactable = true;

        if (PlayerPrefs.GetInt("unlockedLevels", 0) >= 10)
            floor10.interactable = true;

        if (PlayerPrefs.GetInt("unlockedLevels", 0) >= 15)
            floor15.interactable = true;

        if (PlayerPrefs.GetInt("unlockedLevels", 0) >= 20)
            floor20.interactable = true;
    }

    //Loads the game from floor 0
    public void StartFloor0()
    {
        PlayerPrefs.SetInt("startingLevel", 0);
        StartCoroutine(FadeOutToLevel(2));
    }

    public void StartFloor5()
    {
        if(PlayerPrefs.GetInt("unlockedLevels", 0) >= 5)
        {
            PlayerPrefs.SetInt("startingLevel", 4);
            StartCoroutine(FadeOutToLevel(2));
        }
    }

    public void StartFloor10()
    {
        if (PlayerPrefs.GetInt("unlockedLevels", 0) >= 10)
        {
            PlayerPrefs.SetInt("startingLevel", 9);
            StartCoroutine(FadeOutToLevel(2));
        }
    }

    public void StartFloor15()
    {
        if (PlayerPrefs.GetInt("unlockedLevels", 0) >= 15)
        {
            PlayerPrefs.SetInt("startingLevel", 14);
            StartCoroutine(FadeOutToLevel(2));
        }
    }

    public void StartFloor20()
    {
        if (PlayerPrefs.GetInt("unlockedLevels", 0) >= 20)
        {
            PlayerPrefs.SetInt("startingLevel", 19);
            StartCoroutine(FadeOutToLevel(2));
        }
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

    public void ChangeDifficulty()
    {
        switch (PlayerPrefs.GetInt("Difficulty", 1))
        {
            case 0: //casual->standard
                casualDiamonds.SetActive(false);
                standardDiamonds.SetActive(true);
                difficultyButtonText.text = "Standard";
                PlayerPrefs.SetInt("Difficulty", 1);
                break;
            case 1: //standard->hardcore
                standardDiamonds.SetActive(false);
                hardcoreDiamonds.SetActive(true);
                difficultyButtonText.text = "Hardcore";
                PlayerPrefs.SetInt("Difficulty", 2);
                break;
            case 2: //hardcore->casual
                hardcoreDiamonds.SetActive(false);
                casualDiamonds.SetActive(true);
                difficultyButtonText.text = "Casual";
                PlayerPrefs.SetInt("Difficulty", 0);
                break;
            default:
                break;
        }
    }

    //Toggle whether the game is fullscreen or not
    public void ToggleFullscreen()
    {
        //Screen.fullScreen = !Screen.fullScreen;

        if(Screen.fullScreen)
        {
            Screen.SetResolution(1024, 768, false);
            fullscreenText.text = "Windowed";
            PlayerPrefs.SetInt("isFullscreen", 1);
        }
        else
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
            fullscreenText.text = "Fullscreen";
            PlayerPrefs.SetInt("isFullscreen", 0);
        }
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
        ControlSchemeChanged();
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
