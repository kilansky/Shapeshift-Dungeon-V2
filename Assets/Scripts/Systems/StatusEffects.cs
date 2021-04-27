using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusEffects : MonoBehaviour
{
    private float currTime = 0; //CurrTime variable to be accessed by the fireStatus script
    private float timeLeft = 0; //TimeLeft variable to check how much time is left in the current coroutine if it is already running
    private bool isBurning = false; 

    [Header("Effects Particles")]
    public GameObject fireEffect; //Assigns the Fire Effect so the player can see the enemies or themselves burning

    [Header("Player Status Effect Display")]
    public GameObject fireIcon; //Shows an icon to indicate to the player that they are on fire
    public TextMeshProUGUI fireTimeText; //Sets the time remaining to burn for the player;

    private void Start()
    {
        if (fireIcon) //Deactivates the fire icon object on the player (if it exists)
            fireIcon.SetActive(false);
    }

    /// <summary>
    /// Function to start the Enumerator Coroutine to deal damage over a set period of time which will be specified by the source. - AHL (3/29/21)
    /// </summary>
    public void fireStatus(float time)
    {
        //Checks if the Coroutine isn't already running and then starts the coroutine
        if(currTime == 0 && !isBurning)
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
        timeLeft = duration - currTime; //Adjusts time left to show how much time is remaining (aka how much damage is left for the enemy to take)

        //While the tracker is less than the duration the function will run and every second deal a single damage to the player or the enemy that this script is attached to.
        while (currTime < duration)
        {
            isBurning = true;
            fireEffect.SetActive(true); //Activates the fire effect object on the object

            if(fireIcon) //Activates the fire icon object on the player (if it exists)
                fireIcon.SetActive(true); 

            if(fireTimeText) //Sets the text (if it exists) to show the time remaining of the fire duration
                fireTimeText.text = (duration - currTime).ToString();

            yield return new WaitForSeconds(1); //Waits for a single second before checking the while loop again

            //If the object is an enemy (Contains the enemy base script) than deal the damage
            if (GetComponent<EnemyBase>())
                GetComponent<EnemyBase>().FireDamage(1);

            //If the object is the palyer than deal the damage
            if (GetComponent<PlayerHealth>())
                PlayerHealth.Instance.FireDamage(1);

            currTime++; //Adds 1 to the current time to help with the while statement

            timeLeft = duration - currTime; //Adjusts time left to show how much time is remaining (aka how much damage is left for the enemy to take)

            //print("This object: " + gameObject.name + " has this much time remaining for the fire status effect: " + timeLeft);
        }

        isBurning = false;

        if (fireTimeText) //Sets the text (if it exists) to show the time remaining of the fire duration
            fireTimeText.text = (duration - currTime).ToString();

        yield return new WaitForSeconds(0.5f); //Waits for a single second before checking the while loop again

        if(!isBurning)
        {
            fireEffect.SetActive(false); //Deactivates the fire effect on the current object

            if (fireIcon) //Deactivates the fire icon object on the player (if it exists)
                fireIcon.SetActive(false);
        }

        
        currTime = 0; //Resets timer to 0 to start the coroutine up again for the next attack
    }
}
