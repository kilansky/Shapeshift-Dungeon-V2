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

    private Transform spawner;
    private GameObject laser;

    private void Start()
    {
        spawner = transform.GetChild(0);

        laser = Instantiate(blueLaser, spawner.position, spawner.rotation, transform);
    }
}
