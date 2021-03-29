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

    public float destroyTime = 1.5f;

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}
