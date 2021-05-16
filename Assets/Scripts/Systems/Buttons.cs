using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Buttons : MonoBehaviour
{
    public GameObject[] buttonsArray;
    public EventSystem eventSystem;
    //public GameObject optionsPanel;

    [HideInInspector] public int currButtonIndex = 0;

    public void SetSelectedButton()
    {
        ClearSelectedButtons();

        //Set a new selected button
        currButtonIndex = 0;
        eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(buttonsArray[currButtonIndex]);

        if (EventSystem.current.currentSelectedGameObject.GetComponent<Slider>())
        {
            Slider thisSlider = EventSystem.current.currentSelectedGameObject.GetComponent<Slider>();
            thisSlider.transform.GetChild(2).GetChild(0).localScale *= 1.5f;
        }
    }

    public void ClearSelectedButtons()
    {
        //Clear selected buttons
        eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }

    public void NextButton()
    {
        if (EventSystem.current.currentSelectedGameObject.GetComponent<Slider>())
        {
            Slider thisSlider = EventSystem.current.currentSelectedGameObject.GetComponent<Slider>();
            thisSlider.transform.GetChild(2).GetChild(0).localScale /= 1.5f;
        }

        do //Skip over buttons that are not interactable
        {
            currButtonIndex++;
            if (currButtonIndex > buttonsArray.Length - 1)
                currButtonIndex = 0;

            Debug.Log("currButtonIndex " + currButtonIndex);
            Debug.Log("buttonsArray[currButtonIndex]: " + buttonsArray[currButtonIndex]);

        } while (buttonsArray[currButtonIndex].GetComponent<Button>() && !buttonsArray[currButtonIndex].GetComponent<Button>().interactable);

        eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(buttonsArray[currButtonIndex]);

        if (EventSystem.current.currentSelectedGameObject.GetComponent<Slider>())
        {
            Slider thisSlider = EventSystem.current.currentSelectedGameObject.GetComponent<Slider>();
            thisSlider.transform.GetChild(2).GetChild(0).localScale *= 1.5f;
        }
    }

    public void PreviousButton()
    {
        if (EventSystem.current.currentSelectedGameObject.GetComponent<Slider>())
        {
            Slider thisSlider = EventSystem.current.currentSelectedGameObject.GetComponent<Slider>();
            thisSlider.transform.GetChild(2).GetChild(0).localScale /= 1.5f;
        }

        do //Skip over buttons that are not interactable
        {
            currButtonIndex--;
            if (currButtonIndex < 0)
                currButtonIndex = buttonsArray.Length - 1;

            Debug.Log("currButtonIndex " + currButtonIndex);
            Debug.Log("buttonsArray[currButtonIndex]: " + buttonsArray[currButtonIndex]);

        } while (buttonsArray[currButtonIndex].GetComponent<Button>() && !buttonsArray[currButtonIndex].GetComponent<Button>().interactable);

        eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(buttonsArray[currButtonIndex]);

        if (EventSystem.current.currentSelectedGameObject.GetComponent<Slider>())
        {
            Slider thisSlider = EventSystem.current.currentSelectedGameObject.GetComponent<Slider>();
            thisSlider.transform.GetChild(2).GetChild(0).localScale *= 1.5f;
        }
    }

    public void SubmitButton()
    {
        buttonsArray[currButtonIndex].GetComponent<Button>().onClick.Invoke();
    }

    public void ContinueGame()
    {
        PlayerController.Instance.Unpause();
    }

    public void OptionsMenu()
    {
        //optionsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadCredits()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(3);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(2);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
