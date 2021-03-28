using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffects : MonoBehaviour
{
    /// <summary>
    /// Function to start the Enumerator Coroutine to deal damage over a set period of time which will be specified by the source. - AHL (3/23/21)
    /// </summary>
    public void fireStatus(int time)
    {
        StartCoroutine(startFireStatus(time));
    }

    /// <summary>
    /// Deals specific damage over time to the enemy/player - AHL (3/23/21)
    /// </summary>
    IEnumerator startFireStatus(int duration)
    {
        int tracker = 0; //Tracker to make sure that the ticks only happen a certain amount of time as specified by the duration

        //While the tracker is less than the duration the function will run and every second deal a single damage to the player or the enemy that this script is attached to.
        while (tracker < duration)
        {
            //If the object is an enemy (Contains the enemy base script) than deal the damage
            if (this.GetComponent<EnemyBase>())
                GetComponent<EnemyBase>().Damage(1);

            //If the object is the palyer than deal the damage
            if (this.GetComponent<PlayerHealth>())
                GetComponent<PlayerHealth>().Damage(1);
            
            tracker++;

            yield return new WaitForSeconds(1); //Waits for a single second before checking the while loop again
        }

    }
}
