using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDispenser : MonoBehaviour
{
    /*
     * Script: LaserDispenser
     * Programmer: Justin Donato
     * Description: Handles behaviors of pedestals
     * Date Created: 3/23/2021
     * Date Last Edited: 3/29/2021
     */

    [Header("Attacking Variables")]
    public fireTypes laserType;
    public mode firingMode;
    public float timeOn = 1f;
    public float timeOff = 1f;
    public float startupTime = 1f;

    [Header("Laser GameObjects")]
    public GameObject redLaser;
    public GameObject blueLaser;
    public GameObject greenLaser;
    public GameObject startupLaser;
    public GameObject laserStartupEffect;

    [Header("Pointers")]
    public GameObject laserSpawnpoint;

    public enum mode
    {
        constant,
        alternating
    }   

    [HideInInspector]public GameObject laser;
    private GameObject startLaser;

    public enum fireTypes
    {
        red,
        green,
        blue
    }

    private void Start()
    {
        laser = Instantiate(GetLaserColor(), laserSpawnpoint.transform.position, laserSpawnpoint.transform.rotation, transform);
        startLaser = Instantiate(startupLaser, laserSpawnpoint.transform.position, laserSpawnpoint.transform.rotation, transform);

        //Sets the lasers parent game object to this one to make the Damage Tracker acquire the Crystal enemy as the correct game object for tracking
        laser.GetComponent<Laser>().parentObject = transform.root.gameObject;
        startLaser.GetComponent<Laser>().parentObject = transform.root.gameObject;

        laser.SetActive(false);
        startLaser.SetActive(false);
    }

    [ContextMenu("StartLaser")]
    private void StartLaser()
    {
        ToggleLaser(true);
    }

    /// <summary>
    /// Either enables or disables the laser spawning tile
    /// </summary>
    /// <param name="enabled"></param>
    public void ToggleLaser(bool enabled)
    {
        if(enabled)//Start Laser
        {   
            if(firingMode == mode.constant)
            {
                StartCoroutine(StartConstantLaser());
            }
            else
                StartCoroutine(LaserCycle());
        }
        else//Stop Laser
        {
            if(firingMode == mode.alternating)
                StopCoroutine(LaserCycle());

            if(laser)
                laser.SetActive(false);
        }       
    }

    private IEnumerator StartConstantLaser()
    {
        //Laser Charge/Startup effects
        startLaser.SetActive(true);
        LineRenderer lineRenderer = startLaser.transform.GetChild(1).GetComponent<LineRenderer>();
        Gradient lineRendererGradient = new Gradient();
        float alpha = 0f;
        laserStartupEffect.SetActive(true);

        float effectScale = 0.5f;
        laserStartupEffect.transform.localScale = new Vector3(effectScale, effectScale, effectScale);

        float timeElapsed = 0f;
        while(timeElapsed < startupTime)
        {
            //Fade in startup laser
            alpha = Mathf.Lerp(0f, 0.5f, timeElapsed / startupTime);
            lineRendererGradient.SetKeys
            (
                lineRenderer.colorGradient.colorKeys,
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 1f) }
            );
            lineRenderer.colorGradient = lineRendererGradient;

            //Rotate 2D startup effect
            laserStartupEffect.transform.Rotate(0f, 0f, 60f * Time.deltaTime);

            //Scale up 2D startup effect
            effectScale = Mathf.Lerp(1f, 2.5f, timeElapsed / startupTime);
            laserStartupEffect.transform.localScale = new Vector3(effectScale, effectScale, effectScale);

            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //Laser Begins Firing
        startLaser.SetActive(false);
        laserStartupEffect.SetActive(false);
        laser.SetActive(true);
    }

    /// <summary>
    /// Toggles the laser on and off at a set interval
    /// </summary>
    /// <returns></returns>
    private IEnumerator LaserCycle()
    {
        //Laser Charge/Startup effects
        laserStartupEffect.SetActive(true);
        yield return new WaitForSeconds(startupTime);

        //Laser Begins Firing
        laser.SetActive(true);
        yield return new WaitForSeconds(timeOn);

        //Laser Ends Firing
        laser.SetActive(false);

        //Laser waits to start up again
        yield return new WaitForSeconds(timeOff);
        StartCoroutine(LaserCycle());
    }

    /// <summary>
    /// Determines which laser color to spawn based on a set enum
    /// </summary>
    /// <returns></returns>
    private GameObject GetLaserColor()
    {
        if (laserType == fireTypes.red)
            return redLaser;
        else if (laserType == fireTypes.green)
            return greenLaser;
        else
            return blueLaser;
    }
}
