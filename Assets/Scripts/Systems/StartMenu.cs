using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StartMenu : MonoBehaviour
{
    //public GameObject buttonsPanel;
    //public GameObject[] buttons;
    //public EventSystem eventSystem;

    public GameObject startCanvas;
    public GameObject playCanvas;
    public float menuTransitionTime;

    public Image blackScreenOverlay;
    public float fadeInTime = 2f;
    public float fadeOutTime = 2f;

    private float startPos;
    private float endPos;

    // Start is called before the first frame update
    void Start()
    {
        startCanvas.SetActive(true);
        playCanvas.SetActive(false);

        startPos = transform.position.y;
        endPos = transform.position.y - 6f;

        StartCoroutine(FadeFromBlack());
    }

    public void PlayGame()
    {
        //StartCoroutine(FadeToBlack(2));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator TransitionMenu()
    {
        float timeElapsed = 0;
        float currPos = startPos;
        while (timeElapsed < (menuTransitionTime / 2))
        {
            currPos = Mathf.Lerp(startPos, endPos, (menuTransitionTime / 2) / timeElapsed);
            transform.position = new Vector3(transform.position.x, currPos, transform.position.z);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = new Vector3(transform.position.x, endPos, transform.position.z);
    }

    private IEnumerator FadeFromBlack()
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

    private IEnumerator FadeToBlack(int sceneToLoad)
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

        SceneManager.LoadScene(sceneToLoad);
    }
}
