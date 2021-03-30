using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffects : MonoBehaviour
{
    private float currTime = 0; //CurrTime variable to be accessed by the fireStatus script
    private float timeLeft = 0; //TimeLeft variable to check how much time is left in the current coroutine if it is already running

    public GameObject fireEffect;

    /// <summary>
    /// Function to start the Enumerator Coroutine to deal damage over a set period of time which will be specified by the source. - AHL (3/29/21)
    /// </summary>
    public void fireStatus(float time)
    {
        //Checks if the Coroutine isn't already running and then starts the coroutine
        if(currTime == 0)
            StartCoroutine(startFireStatus(time));
    
        //If the Corutine is already running
        else
        {
            //Checks if the current time left is less then the new fire status time then adjusts the current time to set it to the current fire status time
            if (timeLeft < time)
                currTime -= (time - timeLeft);
        }
    }

    /// <summary>
    /// Deals specific damage over time to the enemy/player - AHL (3/29/21)
    /// </summary>
    IEnumerator startFireStatus(float duration)
    {
        currTime = 0; //Tracker to make sure that the ticks only happen a certain amount of time as specified by the duration

        fireEffect.SetActive(true);

        //While the tracker is less than the duration the function will run and every second deal a single damage to the player or the enemy that this script is attached to.
        while (currTime < duration)
        {
            //If the object is an enemy (Contains the enemy base script) than deal the damage
            if (GetComponent<EnemyBase>())
                GetComponent<EnemyBase>().Damage(1);

            //If the object is the palyer than deal the damage
            if (GetComponent<PlayerHealth>())
                GetComponent<PlayerHealth>().Damage(1);

            currTime++; //Adds 1 to the current time to help with the while statement

            timeLeft = duration - currTime; //Adjusts time left to show how much time is remaining (aka how much damage is left for the enemy to take)

            print("This object: " + gameObject.name + " has this much time remaining for the fire status effect: " + timeLeft);

            yield return new WaitForSeconds(1); //Waits for a single second before checking the while loop again
        }

        fireEffect.SetActive(false);
    }
}
