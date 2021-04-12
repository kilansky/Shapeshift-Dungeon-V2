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

    [Header("Laser GameObjects")]
    public GameObject redLaser;
    public GameObject blueLaser;
    public GameObject greenLaser;

    [Header("Pointers")]
    public GameObject laserSpawnpoint;

    public enum mode
    {
        constant,
        alternating
    }   

    [HideInInspector]public GameObject laser;
    public enum fireTypes
    {
        red,
        green,
        blue
    }

    private void Start()
    {
        laser = Instantiate(GetLaserColor(), laserSpawnpoint.transform.position, laserSpawnpoint.transform.rotation, transform);

        //Sets the lasers parent game object to this one to make the Damage Tracker acquire the Crystal enemy as the correct game object for tracking
        laser.GetComponent<Laser>().parentObject = transform.root.gameObject;

        laser.SetActive(false);
    }

    /// <summary>
    /// Either enables or disables the laser spawning tile
    /// </summary>
    /// <param name="enabled"></param>
    public void ToggleLaser(bool enabled)
    {
        if(enabled)
        {   
            if(firingMode == mode.constant)
            {
                laser.SetActive(true);
            }
            else
                StartCoroutine(LaserCycle());
        }
        else
        {
            if(firingMode == mode.alternating)
                StopCoroutine(LaserCycle());

            if(laser)
                laser.SetActive(false);
        }       
    }

    /// <summary>
    /// Toggles the laser on and off at a set interval
    /// </summary>
    /// <returns></returns>
    private IEnumerator LaserCycle()
    {
        laser.SetActive(true);

        yield return new WaitForSeconds(timeOn);

        laser.SetActive(false);

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
