using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMonster : MonoBehaviour
{
    /*
     * Script: GemMonster
     * Programmer: Justin Donato
     * Description: Holds info on if enemy is a gem monster
     * Date Created: 3/28/2021
     * Date Last Edited: 3/29/2021
     */

    public bool isGemMonster = false;
    public GameObject gemDisplay;

    public void SetGemMonster()
    {
        isGemMonster = true;
        gemDisplay.SetActive(true);
    }
}
