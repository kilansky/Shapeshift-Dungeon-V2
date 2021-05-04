using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DangerTiles : MonoBehaviour
{
    public float flashTime = 1f;
    public float numberOfFlashes = 3;
    public float maximumAlpha = 0.7f;
    public LayerMask lookForLayer;
    private Image warningSquare;

    private void Start()
    {
        warningSquare = GetComponent<Image>();

        //Set starting alpha value to 0
        var tempColor = warningSquare.color;
        tempColor.a = 0;
        warningSquare.color = tempColor;
    }

    public void ParentToTile()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + new Vector3(0, 1f, 0), Vector3.down, out hit, 10f, lookForLayer)) //Sends a raycast to look for an object below this one
        {
            if (hit.transform.gameObject.GetComponent<Tile>()) //If the raycast finds an object, this finds out if that object is a tile
                transform.parent.parent = hit.transform; //Parent this to the tile below it
        }
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

        yield return new WaitForSeconds(4);
        Destroy(transform.parent);
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
