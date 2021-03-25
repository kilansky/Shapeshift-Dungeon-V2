using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverButtons;

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

    public IEnumerator WaitToDisplayReview()
    {
        yield return new WaitForSecondsRealtime(2f);

        gameOverButtons.SetActive(true);

        if (HUDController.Instance.ShowLevelReview)
            HUDController.Instance.ShowLevelReviewPanel();
    }
}
