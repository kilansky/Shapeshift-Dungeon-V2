using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Buttons : MonoBehaviour
{
    public GameObject[] buttons;
    public EventSystem eventSystem;

    private int currButtonIndex = 0;

    public void SetSelectedButton()
    {
        ClearSelectedButtons();

        //Set a new selected button
        currButtonIndex = 0;
        eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(buttons[currButtonIndex]);
    }

    public void ClearSelectedButtons()
    {
        //Clear selected buttons
        eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null);
    }

    public void NextButton()
    {
        currButtonIndex++;
        if (currButtonIndex > buttons.Length - 1)
            currButtonIndex = 0;

        eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(buttons[currButtonIndex]);
    }

    public void PreviousButton()
    {
        currButtonIndex--;
        if (currButtonIndex < 0)
            currButtonIndex = buttons.Length - 1;

        eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(buttons[currButtonIndex]);
    }

    public void SubmitButton()
    {
        buttons[currButtonIndex].GetComponent<Button>().onClick.Invoke();
    }

    public void ContinueGame()
    {
        PlayerController.Instance.Unpause();
    }

    public void QuitGame()
    {
        Application.Quit();
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
