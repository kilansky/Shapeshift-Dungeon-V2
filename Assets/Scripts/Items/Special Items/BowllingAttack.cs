using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowllingAttack : MonoBehaviour
{
    //Variable Initialization/Declaration
    public float damage; //Damage variable to adjust the damage the enemy takes when it gets hit
    //public float destroyTime; //This is the time that it will take before the bowling ball gets destroyed
    public float speed; //The speed that the bowling ball will go at
    private bool isOverPit = false; //Bool to change the transform.translate to down if it is over a pit by setting this bool to true

    private void Start()
    {
        Destroy(gameObject, 8f);
    }

    /// <summary>
    /// OnTrigger so when it hits an enemy it will damage them - AHL (3/4/21)
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        //If the other object is Monster (Contains the enemy base script) then go on with the rest of the damage
        if (other.GetComponent<EnemyBase>())
        {
            StartCoroutine(other.GetComponent<EnemyBase>().EnemyKnockBack()); 
            other.GetComponent<EnemyBase>().Damage(damage);
            AudioManager.Instance.Play("BowlingStrike");
        }

        //If the bowling ball hits a wall layer then it deletes itself
        if (other.gameObject.layer == 9)
        {
            Destroy(gameObject);
        }
    }

    //Update is called once a frame
    private void Update()
    {
        //If the ball is not over a pit then it moves forward
        if (!isOverPit)
            transform.Translate(Vector3.forward * speed * Time.deltaTime); //Every frame the bowling ball will move forward

        //If the ball is over a pit then it moves downward into the pit itself
        else
        {
            //Every frame the bowling ball will move downward
            transform.Translate(Vector3.down * speed * Time.deltaTime); 

            //And a bit forward
            transform.Translate((Vector3.forward * speed * Time.deltaTime) / 2.25f);
        }
            
        
        
        //And rotate forward - Sky (3/29/21)
        //transform.RotateAround(transform.position, Vector3.right, speed * Time.deltaTime);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator ballOverPit()
    {
        //A minor delay so the ball will be fully over the pit
        yield return new WaitForSeconds(0.05f);
        
        //Adjusts the isoverpit to true so the ball falls down it
        isOverPit = true;
    }
}
