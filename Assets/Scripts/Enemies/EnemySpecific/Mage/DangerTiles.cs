using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DangerTiles : MonoBehaviour
{
    public float flashTime = 1f;
    public float numberOfFlashes = 3;
    public float maximumAlpha = 0.7f;
    private Image warningSquare;

    private void Start()
    {
        warningSquare = GetComponent<Image>();

        //Set starting alpha value to 0
        var tempColor = warningSquare.color;
        tempColor.a = 0;
        warningSquare.color = tempColor;
    }

    public void StartFlashing()
    {
        StartCoroutine(DangerFlashing());
    }

    private IEnumerator DangerFlashing()
    {
        for (int i = 0; i < numberOfFlashes; i++)
        {
            //Fade In
            StartCoroutine(FadeAlphaValues(0, maximumAlpha));
            yield return new WaitForSeconds((flashTime / 2));

            //Fade Out
            if(i+1 != numberOfFlashes)//Don't fade out on the final flash
            {
                StartCoroutine(FadeAlphaValues(maximumAlpha, 0));
                yield return new WaitForSeconds((flashTime / 2));
            }
        }
    }

    private IEnumerator FadeAlphaValues(float startValue, float endValue)
    {
        float timeElapsed = 0;
        var tempColor = warningSquare.color;

        while (timeElapsed < (flashTime/2))
        {
            tempColor.a = Mathf.Lerp(startValue, endValue, timeElapsed / (flashTime / 2));
            warningSquare.color = tempColor;

            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
