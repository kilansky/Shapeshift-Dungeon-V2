using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowllingAttack : MonoBehaviour
{
    //Variable Initialization/Declaration
    public float damage; //Damage variable to adjust the damage the enemy takes when it gets hit
    //public float destroyTime; //This is the time that it will take before the bowling ball gets destroyed
    public float speed; //The speed that the bowling ball will go at

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
        }
    }

        /*if (other.gameObject.layer == 9 || other.gameObject.layer == 10)
            Destroy(gameObject);

    //Destroy when colliding with Environment Layer or the Wall Layer
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 9 || collision.gameObject.layer == 10)
            Destroy(gameObject);
    }*/

    //Update is called once a frame
    private void Update()
    {
        //Every frame the bowling ball will move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        //And rotate forward - Sky (3/29/21)
        //transform.RotateAround(transform.position, Vector3.right, speed * Time.deltaTime);
    }
}
