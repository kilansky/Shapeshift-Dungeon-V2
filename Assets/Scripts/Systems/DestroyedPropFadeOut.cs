﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedPropFadeOut : MonoBehaviour
{
    public GameObject rootProp;
    public Material gemMat;
    public bool isPot;

    private Renderer renderer;

    void Start()
    {
        //Get renderer and set gem material if the root prop will spawn a gem
        renderer = GetComponent<Renderer>();

        if (rootProp.GetComponent<DestructibleProp>().HasGem)
            renderer.material = gemMat;

        //Start coroutine to fade out and disable this
        StartCoroutine(DestroySelf());
    }

    private IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(8f);

        float timeElaped = 0f;
        float timeToFade = 2f;

        while (timeElaped < timeToFade)
        {
            Color newColor;
            if (!isPot)
                newColor = new Color(1, 1, 1, Mathf.Lerp(1, 0, timeElaped / timeToFade));
            else
                newColor = new Color(0.624f, 0.5003502f, 0.2875576f, Mathf.Lerp(1, 0, timeElaped / timeToFade));

            renderer.material.color = newColor;

            timeElaped += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        renderer.material.color = new Color(1, 1, 1, 0);
    }
}
