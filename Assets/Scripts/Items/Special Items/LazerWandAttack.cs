using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerWandAttack : MonoBehaviour
{
    //Variable Initialization/Declaration
    public float damage; //Damage variable to adjust the damage the enemy takes when it gets hit

    private void Start()
    {
        Destroy(gameObject, 0.3f);
    }

    /// <summary>
    /// OnTrigger so when it hits an enemy it will damage them - AHL (4/13/21)
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        //If the other object is Monster (Contains the enemy base script) then go on with the rest of the damage then destroys itself
        if (other.GetComponent<EnemyBase>())
        {
            other.GetComponent<EnemyBase>().Damage(damage);
        }
    }

    //Update is called once a frame
    private void Update()
    {

    }
}
