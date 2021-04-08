using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Buttons : MonoBehaviour
{
    public GameObject buttonsPanel;
    public GameObject[] buttons;
    public EventSystem eventSystem;

    private int buttonIndex;

    private void Start()
    {
        //Clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        
        //Set a new selected object
        buttonIndex = 0;
        //buttons[buttonIndex].GetComponent<Button>().Select();
        eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(buttons[buttonIndex]);
    }

    public void NextButton()
    {
        buttonIndex++;
        if (buttonIndex > buttons.Length - 1)
            buttonIndex = 0;

        //buttons[buttonIndex].GetComponent<Button>().Select();

        eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(buttons[buttonIndex]);
        Debug.Log("NextButton Performed");
    }

    public void PreviousButton()
    {
        buttonIndex--;
        if (buttonIndex < 0)
            buttonIndex = buttons.Length - 1;

        //buttons[buttonIndex].GetComponent<Button>().Select();

        eventSystem.GetComponent<EventSystem>().SetSelectedGameObject(buttons[buttonIndex]);
        Debug.Log("PreviousButton Performed");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void FormFeedback()
    {
        Application.OpenURL("https://docs.google.com/forms/d/1yeWTvuf43eci_y8Tj1MR-Tj1UTHPfomzOjMlrw7A2mM/edit");
    }

    public void DisableLevelReview()
    {
        HUDController.Instance.ShowLevelReview = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    //Show that the player died, wait, then show level review panel
    public IEnumerator WaitToDisplayReview()
    {
        yield return new WaitForSecondsRealtime(2f);

        buttonsPanel.SetActive(true);

        if (HUDController.Instance.ShowLevelReview)
            HUDController.Instance.ShowLevelReviewPanel();
    }
}
