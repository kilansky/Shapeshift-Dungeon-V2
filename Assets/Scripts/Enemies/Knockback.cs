using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    //this script will handle all instances of knockback needed in game
    //will do this by using addForce to the rigidbody of objects
    //movement and all other functions should be disabled while being knocked back

    //knockback will be applied on basic attacks, 
    //bowling ball, laser/fire wand

    public float knockBackStrength = 10f;

    public Vector3 knockbackDirection;

    public EnemyBase enemy;

    private Vector3 OGposition;

    private Rigidbody rb;

    /*private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.collider.GetComponent<Rigidbody>();
        Debug.Log("I hit something");

        //straight line knockback
        if(rb != null)
        {
            Vector3 direction = (collision.transform.position - transform.position).normalized;
            
            //don't knock them back in the air
            direction.y = 0;

            rb.AddForce(direction.normalized * knockBackStrength, ForceMode.Impulse);
        }
    }*/

    public void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        enemy = other.GetComponent<EnemyBase>();
        //Debug.Log("enemy is equal to " + enemy.name);

        //enemy = FindObjectOfType<EnemyBase>();
        rb = other.GetComponent<Rigidbody>();
        //Rigidbody rb = other.GetComponent<EnemyBase>().aliveGO.GetComponent<Rigidbody>();
        //Rigidbody rb = other.GetComponent<EnemyBase>().GetComponent<Rigidbody>();
        //Debug.Log("rb is equal to " + rb);

        //straight line knockback
        if (other.gameObject.CompareTag("Monster"))
        {
            //Vector3 direction = (other.transform.position - transform.position).normalized;
            enemy.StartCoroutine(EnemyKnockBack());
            //rb.AddForce(knockbackDirection.normalized * knockBackStrength, ForceMode.Impulse);
        }
    }

    public IEnumerator EnemyKnockBack()
    {
        Debug.Log("knockback script called");
        //Debug.Log("RB is equal to " + enemy.RB);

        //OGposition = enemy.RB.transform.position;
        Debug.Log("OG chillin at " + OGposition);

        enemy.isKnockedBack = true;
        enemy.agent.enabled = false;
        enemy.agent.isStopped = true;
        //enemy.RB.isKinematic = false;

        //don't knock them back in the air
        
        //TODO: double check the logic here should be enemy - knockbacker object
        knockbackDirection = (enemy.transform.position - gameObject.transform.position).normalized;
        knockbackDirection.y = 0;
        Debug.Log("obects involved are " + enemy.transform + ", " + gameObject.transform);
        
        //enemy.RB.velocity = knockbackDirection * knockBackStrength;
        //enemy.RB.AddForce(knockbackDirection.normalized * knockBackStrength, ForceMode.Impulse);

        //enemy.transform.position = enemy.RB.transform.localPosition;
        
        //enemy.RB.transform.position = OGposition;
        //Debug.Log("OG now chillin at " + OGposition);
        //Debug.Log("the RB is at " + enemy.RB.transform.position);

        yield return new WaitForSeconds(0.5f);

        enemy.isKnockedBack = false;
        enemy.agent.enabled = true;
        enemy.agent.isStopped = false;
        //enemy.RB.isKinematic = true;
    }

}
