using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    /*
     * Script: Laser
     * Programmer: Justin Donato and Joe Warren
     * Description: Handles behaviors of laser objects
     * Date Created: 3/23/2021
     * Date Last Edited: 3/23/2021
     */

    /// <summary>
    /// Sets the scales of all the vfx effects based on a given percentage
    /// </summary>
    /// <param name="percent"></param>
    public void SetVFXScale(float percent)
    {
        float scaleFactor = percent / 100f;

        foreach(Transform child in transform)
        {
            child.transform.localScale = new Vector3(1, 1, 1) * scaleFactor;
        }
    }
}
