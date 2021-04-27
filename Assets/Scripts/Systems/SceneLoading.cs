using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Handles transitioning between the start menu and the game scene, and shows a loading bar
public class SceneLoading : MonoBehaviour
{
    public Slider loadingBar;
    public Transform loadingSpinner;
    public float progressAnimationMultiplier = 0.25f;

    public Image blackScreenOverlay;
    public float fadeInTime = 1f;
    public float fadeOutTime = 1.5f;

    private AsyncOperation gameLevel;
    private float progress = 0f;

    void Start()
    {
        loadingBar.value = 0;
        StartCoroutine(FadeFromBlack());
    }

    IEnumerator LoadAsyncOperation()
    {
        yield return new WaitForSeconds(1f);

        gameLevel = SceneManager.LoadSceneAsync(2);
        gameLevel.allowSceneActivation = false;

        while (progress < 1f)
        {
            progress = Mathf.Clamp01(gameLevel.progress / 0.9f);
            //progress = Mathf.MoveTowards(progress, 1, progressAnimationMultiplier * Time.deltaTime);
            loadingBar.value = progress;
            loadingSpinner.Rotate(0f,0f,100f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(FadeToBlack());
    }

    private IEnumerator FadeFromBlack()
    {
        float alpha = 1;
        blackScreenOverlay.color = new Color(0, 0, 0, alpha);
        float timeElapsed = 0;

        while (timeElapsed < fadeInTime)
        {
            alpha = Mathf.Lerp(1, 0, timeElapsed / fadeInTime);
            blackScreenOverlay.color = new Color(0, 0, 0, alpha);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(LoadAsyncOperation());
    }

    private IEnumerator FadeToBlack()
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
        blackScreenOverlay.color = new Color(0, 0, 0, 0);

        gameLevel.allowSceneActivation = true;
    }
}
