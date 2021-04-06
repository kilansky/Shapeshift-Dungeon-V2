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

    private void Start()
    {
        //Clear selected object
        //EventSystem.current.SetSelectedGameObject(null);
        //Set a new selected object
        //EventSystem.current.SetSelectedGameObject(buttons[0]);
    }

    public void NextButton()
    {

    }

    public void PreviousButton()
    {

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
