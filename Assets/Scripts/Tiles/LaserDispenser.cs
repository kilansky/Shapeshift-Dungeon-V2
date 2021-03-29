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
     * Date Last Edited: 3/23/2021
     */

    [Header("Laser GameObjects")]
    public GameObject redLaser;
    public GameObject blueLaser;
    public GameObject greenLaser;

    [Header("Attacking Variables")]
    public float timeOn = 1f;
    public float timeOff = 1f;

    private Transform spawner;
    private GameObject laser;

    private void Start()
    {
        spawner = transform.GetChild(0);

        BeginLaser();
    }

    public void BeginLaser()
    {
        laser = Instantiate(blueLaser, spawner.position, spawner.rotation, transform);
        StartCoroutine(laserCycle());
    }

    private IEnumerator laserCycle()
    {
        laser.SetActive(true);

        yield return new WaitForSeconds(timeOn);

        laser.SetActive(false);

        yield return new WaitForSeconds(timeOff);
        StartCoroutine(laserCycle());
    }
}
