using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDeletion : MonoBehaviour
{
    /*
     * Script: TimedDeletion
     * Programmer: Justin Donato
     * Description: General script for any object that needs to be deleted after a certain time
     * Date Created: 3/28/2021
     * Date Last Edited: 3/28/2021
     */

    [Header("Mandatory")]
    public float destroyTime = 1.5f;

    [Header("Optional")]
    public Light pointLight;

    private void Start()
    {
        if (pointLight)
            StartCoroutine(FadeOutPointlight());

        Destroy(gameObject, destroyTime);
    }

    private IEnumerator FadeOutPointlight()
    {
        float timeElapsed = 0f;
        float startingLightIntensity = pointLight.intensity;

        while (timeElapsed < destroyTime)
        {
            pointLight.intensity = Mathf.Lerp(startingLightIntensity, 0, timeElapsed / destroyTime);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
