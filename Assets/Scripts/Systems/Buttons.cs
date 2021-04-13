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
        yield return new WaitForSecondsRealtime(1f);

        //Send the damage the player took as an email - only if in build, not editor
        if (!Application.isEditor)
        {
            //print("We are running this from inside of the editor!");
            PlayerController.Instance.GetComponent<DamageTracker>().DisplayDamage();
        }

        yield return new WaitForSecondsRealtime(1f);

        buttonsPanel.SetActive(true);

        if (HUDController.Instance.ShowLevelReview)
            HUDController.Instance.ShowLevelReviewPanel();
    }
}
